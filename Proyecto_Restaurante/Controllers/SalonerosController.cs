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
    public class SalonerosController : Controller
    {
        private readonly RestauranteDalyContext _context;

        public SalonerosController(RestauranteDalyContext context)
        {
            _context = context;
        }

        // GET: Saloneros
        public async Task<IActionResult> Index()
        {
            return View(await _context.Saloneros.ToListAsync());
        }

        // GET: Saloneros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salonero = await _context.Saloneros
                .FirstOrDefaultAsync(m => m.SaloneroId == id);
            if (salonero == null)
            {
                return NotFound();
            }

            return View(salonero);
        }

        // GET: Saloneros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Saloneros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SaloneroId,Nombre")] Salonero salonero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salonero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salonero);
        }

        // GET: Saloneros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salonero = await _context.Saloneros.FindAsync(id);
            if (salonero == null)
            {
                return NotFound();
            }
            return View(salonero);
        }

        // POST: Saloneros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaloneroId,Nombre")] Salonero salonero)
        {
            if (id != salonero.SaloneroId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salonero);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaloneroExists(salonero.SaloneroId))
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
            return View(salonero);
        }

        // GET: Saloneros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salonero = await _context.Saloneros
                .FirstOrDefaultAsync(m => m.SaloneroId == id);
            if (salonero == null)
            {
                return NotFound();
            }

            return View(salonero);
        }

        // POST: Saloneros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salonero = await _context.Saloneros.FindAsync(id);
            if (salonero != null)
            {
                _context.Saloneros.Remove(salonero);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaloneroExists(int id)
        {
            return _context.Saloneros.Any(e => e.SaloneroId == id);
        }
    }
}
