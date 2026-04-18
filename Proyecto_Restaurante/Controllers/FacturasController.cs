using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Restaurante.Data;
using Proyecto_Restaurante.Models;

namespace Proyecto_Restaurante.Controllers
{
    public class FacturasController : Controller
    {
        private readonly RestauranteDalyContext _context;
        private const decimal TASA_IMPUESTO = 0.13m;

        public FacturasController(RestauranteDalyContext context)
        {
            _context = context;
        }

        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            var restauranteDalyContext = _context.Facturas
                .Include(f => f.Orden);

            return View(await restauranteDalyContext.ToListAsync());
        }

        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.Orden)
                    .ThenInclude(o => o.Mesa)
                .Include(f => f.Orden)
                    .ThenInclude(o => o.Salonero)
                .Include(f => f.Orden)
                    .ThenInclude(o => o.ProductoOrdens)
                        .ThenInclude(po => po.Producto)
                .FirstOrDefaultAsync(m => m.FacturaId == id);

            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // GET: Facturas/Create
        public IActionResult Create()
        {
            ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacturaId,OrdenId,Subtotal,Total,Impuestos,MetodoPago,NumeroFactura")] Factura factura)
        {
            if (factura.Subtotal == null || factura.Subtotal < 0)
            {
                ModelState.AddModelError("Subtotal", "El subtotal es obligatorio y no puede ser negativo.");
            }

            if (factura.Impuestos == null || factura.Impuestos < 0)
            {
                ModelState.AddModelError("Impuestos", "Los impuestos son obligatorios y no pueden ser negativos.");
            }

            if (string.IsNullOrWhiteSpace(factura.NumeroFactura))
            {
                factura.NumeroFactura = $"FAC-{DateTime.Now:yyyyMMddHHmmss}";
            }

            bool numeroFacturaExiste = await _context.Facturas
                .AnyAsync(f => f.NumeroFactura == factura.NumeroFactura);

            if (numeroFacturaExiste)
            {
                ModelState.AddModelError("NumeroFactura", "El número de factura ya existe.");
            }

            if (factura.Subtotal != null && factura.Impuestos != null)
            {
                factura.Total = factura.Subtotal.Value + factura.Impuestos.Value;
            }

            if (!ModelState.IsValid)
            {
                ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", factura.OrdenId);
                return View(factura);
            }

            try
            {
                _context.Add(factura);
                await _context.SaveChangesAsync();

                // Liberar la mesa asociada a la orden facturada
                var orden = await _context.Ordens
                    .FirstOrDefaultAsync(o => o.OrdenId == factura.OrdenId);

                if (orden?.MesaId != null)
                {
                    bool quedanOrdenesActivas = await _context.Ordens
                        .AnyAsync(o => o.MesaId == orden.MesaId &&
                                       !_context.Facturas.Any(f => f.OrdenId == o.OrdenId));

                    var mesa = await _context.Mesas.FindAsync(orden.MesaId);

                    if (mesa != null)
                    {
                        mesa.Estado = quedanOrdenesActivas ? "Ocupada" : "Disponible";
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar la factura.");
                ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", factura.OrdenId);
                return View(factura);
            }
        }

        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", factura.OrdenId);
            return View(factura);
        }

        // POST: Facturas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacturaId,OrdenId,MetodoPago,NumeroFactura")] Factura factura)
        {
            if (id != factura.FacturaId)
            {
                return NotFound();
            }

            if (factura.OrdenId == null)
            {
                ModelState.AddModelError("OrdenId", "Debe seleccionar una orden.");
            }

            decimal subtotalCalculado = 0m;

            if (factura.OrdenId != null)
            {
                var ordenExiste = await _context.Ordens
                    .AnyAsync(o => o.OrdenId == factura.OrdenId);

                if (!ordenExiste)
                {
                    ModelState.AddModelError("OrdenId", "La orden seleccionada no es válida.");
                }
                else
                {
                    subtotalCalculado = await _context.ProductoOrdens
                        .Where(po => po.OrdenId == factura.OrdenId)
                        .SumAsync(po => (po.Cantidad ?? 0) * (po.Precio ?? 0m));

                    if (subtotalCalculado <= 0)
                    {
                        ModelState.AddModelError("OrdenId", "La orden seleccionada no tiene productos para facturar.");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(factura.NumeroFactura))
            {
                factura.NumeroFactura = $"FAC-{DateTime.Now:yyyyMMddHHmmss}";
            }

            var numeroFacturaExiste = await _context.Facturas
                .AnyAsync(f => f.NumeroFactura == factura.NumeroFactura && f.FacturaId != factura.FacturaId);

            if (numeroFacturaExiste)
            {
                ModelState.AddModelError("NumeroFactura", "El número de factura ya existe.");
            }

            factura.Subtotal = subtotalCalculado;
            factura.Impuestos = Math.Round(subtotalCalculado * TASA_IMPUESTO, 2);
            factura.Total = factura.Subtotal + factura.Impuestos;

            if (!ModelState.IsValid)
            {
                ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", factura.OrdenId);
                return View(factura);
            }

            try
            {
                var facturaExistente = await _context.Facturas.FindAsync(id);

                if (facturaExistente == null)
                {
                    return NotFound();
                }

                facturaExistente.OrdenId = factura.OrdenId;
                facturaExistente.MetodoPago = factura.MetodoPago;
                facturaExistente.NumeroFactura = factura.NumeroFactura;
                facturaExistente.Subtotal = factura.Subtotal;
                facturaExistente.Impuestos = factura.Impuestos;
                facturaExistente.Total = factura.Total;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(factura.FacturaId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.Orden)
                .FirstOrDefaultAsync(m => m.FacturaId == id);

            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public async Task<IActionResult> ObtenerSubtotalOrden(int ordenId)
        {
            var ordenExiste = await _context.Ordens
                .AnyAsync(o => o.OrdenId == ordenId);

            if (!ordenExiste)
            {
                return NotFound(new { mensaje = "La orden no existe." });
            }

            var subtotal = await _context.ProductoOrdens
                .Where(po => po.OrdenId == ordenId)
                .SumAsync(po => (po.Cantidad ?? 0) * (po.Precio ?? 0m));

            var impuestos = Math.Round(subtotal * TASA_IMPUESTO, 2);
            var total = subtotal + impuestos;

            return Json(new
            {
                subtotal,
                impuestos,
                total
            });
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.FacturaId == id);
        }
    }
}