using System;
using System.Collections.Generic;
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

        public FacturasController(RestauranteDalyContext context)
        {
            _context = context;
        }

        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            var restauranteDalyContext = _context.Facturas.Include(f => f.Orden);
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

        // POST: Facturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacturaId,OrdenId,Subtotal,Total,Impuestos,MetodoPago,NumeroFactura")] Factura factura)
        {
            if (id != factura.FacturaId)
            {
                return NotFound();
            }

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
                .AnyAsync(f => f.NumeroFactura == factura.NumeroFactura && f.FacturaId != factura.FacturaId);

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
                _context.Update(factura);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(factura.FacturaId))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar la factura.");
                ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", factura.OrdenId);
                return View(factura);
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

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.FacturaId == id);
        }
    }
}
