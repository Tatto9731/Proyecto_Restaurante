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
    public class ProductosOrdenesController : Controller
    {
        private readonly RestauranteDalyContext _context;

        public ProductosOrdenesController(RestauranteDalyContext context)
        {
            _context = context;
        }

        // GET: ProductosOrdenes
        public async Task<IActionResult> Index()
        {
            var restauranteDalyContext = _context.ProductoOrdens.Include(p => p.Orden).Include(p => p.Producto);
            return View(await restauranteDalyContext.ToListAsync());
        }

        // GET: ProductosOrdenes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productoOrden = await _context.ProductoOrdens
                .Include(p => p.Orden)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(m => m.ProductoOrdenId == id);
            if (productoOrden == null)
            {
                return NotFound();
            }

            return View(productoOrden);
        }

        // GET: ProductosOrdenes/Create
        public IActionResult Create()
        {
            ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId");
            return View();
        }

        // POST: ProductosOrdenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoOrdenId,OrdenId,ProductoId,Cantidad,Precio")] ProductoOrden productoOrden)
        {
           
                _context.Add(productoOrden);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", productoOrden.OrdenId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", productoOrden.ProductoId);
            return View(productoOrden);
        }

        // GET: ProductosOrdenes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productoOrden = await _context.ProductoOrdens.FindAsync(id);
            if (productoOrden == null)
            {
                return NotFound();
            }
            ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", productoOrden.OrdenId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", productoOrden.ProductoId);
            return View(productoOrden);
        }

        // POST: ProductosOrdenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoOrdenId,OrdenId,ProductoId,Cantidad,Precio")] ProductoOrden productoOrden)
        {
            if (id != productoOrden.ProductoOrdenId)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(productoOrden);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoOrdenExists(productoOrden.ProductoOrdenId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            ViewData["OrdenId"] = new SelectList(_context.Ordens, "OrdenId", "OrdenId", productoOrden.OrdenId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", productoOrden.ProductoId);
            return View(productoOrden);
        }

        // GET: ProductosOrdenes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productoOrden = await _context.ProductoOrdens
                .Include(p => p.Orden)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(m => m.ProductoOrdenId == id);
            if (productoOrden == null)
            {
                return NotFound();
            }

            return View(productoOrden);
        }

        // POST: ProductosOrdenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productoOrden = await _context.ProductoOrdens.FindAsync(id);
            if (productoOrden != null)
            {
                _context.ProductoOrdens.Remove(productoOrden);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoOrdenExists(int id)
        {
            return _context.ProductoOrdens.Any(e => e.ProductoOrdenId == id);
        }
    }
}
