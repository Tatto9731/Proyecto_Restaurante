using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Restaurante.Data;
using Proyecto_Restaurante.Models;

namespace Proyecto_Restaurante.Controllers
{
    public class OrdenesController : Controller
    {
        private readonly RestauranteDalyContext _context;

        public OrdenesController(RestauranteDalyContext context)
        {
            _context = context;
        }

        // GET: Ordenes
        public async Task<IActionResult> Index()
        {
            var restauranteDalyContext = _context.Ordens
                .Include(o => o.Mesa)
                .Include(o => o.Salonero);

            return View(await restauranteDalyContext.ToListAsync());
        }

        // GET: Ordenes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orden = await _context.Ordens
                .Include(o => o.Mesa)
                .Include(o => o.Salonero)
                .FirstOrDefaultAsync(m => m.OrdenId == id);

            if (orden == null)
            {
                return NotFound();
            }

            return View(orden);
        }

        // GET: Ordenes/Create
        public IActionResult Create()
        {
            var mesasDisponibles = _context.Mesas
                .Where(m => !_context.Ordens.Any(o =>
                    o.MesaId == m.MesaId &&
                    !_context.Facturas.Any(f => f.OrdenId == o.OrdenId)))
                .ToList();

            ViewData["MesaId"] = new SelectList(mesasDisponibles, "MesaId", "NumeroMesa");
            ViewData["SaloneroId"] = new SelectList(_context.Saloneros, "SaloneroId", "Nombre");

            return View();
        }

        // POST: Ordenes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdenId,MesaId,SaloneroId,Fecha")] Orden orden)
        {
            bool mesaTieneOrdenActiva = await _context.Ordens
                .AnyAsync(o => o.MesaId == orden.MesaId &&
                               !_context.Facturas.Any(f => f.OrdenId == o.OrdenId));

            if (mesaTieneOrdenActiva)
            {
                ModelState.AddModelError("MesaId", "La mesa seleccionada ya tiene una orden activa.");
            }

            if (!ModelState.IsValid)
            {
                var mesasDisponibles = _context.Mesas
                    .Where(m => !_context.Ordens.Any(o =>
                        o.MesaId == m.MesaId &&
                        !_context.Facturas.Any(f => f.OrdenId == o.OrdenId))
                        || m.MesaId == orden.MesaId)
                    .ToList();

                ViewData["MesaId"] = new SelectList(mesasDisponibles, "MesaId", "NumeroMesa", orden.MesaId);
                ViewData["SaloneroId"] = new SelectList(_context.Saloneros, "SaloneroId", "Nombre", orden.SaloneroId);
                return View(orden);
            }

            if (orden.Fecha == null)
            {
                orden.Fecha = DateTime.Now;
            }

            _context.Add(orden);
            await _context.SaveChangesAsync();

            var mesa = await _context.Mesas.FindAsync(orden.MesaId);
            if (mesa != null)
            {
                mesa.Estado = "Ocupada";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Ordenes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orden = await _context.Ordens.FindAsync(id);
            if (orden == null)
            {
                return NotFound();
            }

            ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "NumeroMesa", orden.MesaId);
            ViewData["SaloneroId"] = new SelectList(_context.Saloneros, "SaloneroId", "Nombre", orden.SaloneroId);
            return View(orden);
        }

        // POST: Ordenes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrdenId,MesaId,SaloneroId,Fecha")] Orden orden)
        {
            if (id != orden.OrdenId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "NumeroMesa", orden.MesaId);
                ViewData["SaloneroId"] = new SelectList(_context.Saloneros, "SaloneroId", "Nombre", orden.SaloneroId);
                return View(orden);
            }

            try
            {
                _context.Update(orden);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenExists(orden.OrdenId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Ordenes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orden = await _context.Ordens
                .Include(o => o.Mesa)
                .Include(o => o.Salonero)
                .FirstOrDefaultAsync(m => m.OrdenId == id);

            if (orden == null)
            {
                return NotFound();
            }

            return View(orden);
        }

        // POST: Ordenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orden = await _context.Ordens.FindAsync(id);

            if (orden == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var mesaId = orden.MesaId;

            bool tieneFactura = await _context.Facturas
                .AnyAsync(f => f.OrdenId == id);

            if (tieneFactura)
            {
                TempData["Error"] = "No se puede eliminar la orden porque ya tiene una factura asociada.";
                return RedirectToAction(nameof(Index));
            }

            var productosOrden = await _context.ProductoOrdens
                .Where(p => p.OrdenId == id)
                .ToListAsync();

            if (productosOrden.Any())
            {
                _context.ProductoOrdens.RemoveRange(productosOrden);
            }

            _context.Ordens.Remove(orden);
            await _context.SaveChangesAsync();

            if (mesaId.HasValue)
            {
                bool quedanOrdenesActivas = await _context.Ordens
                    .AnyAsync(o => o.MesaId == mesaId &&
                                   !_context.Facturas.Any(f => f.OrdenId == o.OrdenId));

                var mesa = await _context.Mesas.FindAsync(mesaId.Value);
                if (mesa != null)
                {
                    mesa.Estado = quedanOrdenesActivas ? "Ocupada" : "Disponible";
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "La orden fue eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool OrdenExists(int id)
        {
            return _context.Ordens.Any(e => e.OrdenId == id);
        }
    }
}