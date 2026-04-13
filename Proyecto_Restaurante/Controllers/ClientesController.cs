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
    public class ClientesController : Controller
    {
        private readonly RestauranteDalyContext _context;

        public ClientesController(RestauranteDalyContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.ClienteId == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,Nombre,Identificacion,CorreoElectronico,Telefono")] Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Identificacion))
            {
                ModelState.AddModelError("Identificacion", "La identificación es obligatoria.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Identificacion))
            {
                bool identificacionExiste = await _context.Clientes
                    .AnyAsync(c => c.Identificacion == cliente.Identificacion);

                if (identificacionExiste)
                {
                    ModelState.AddModelError("Identificacion", "La identificación ya está registrada.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el cliente.");
                return View(cliente);
            }
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClienteId,Nombre,Identificacion,CorreoElectronico,Telefono")] Cliente cliente)
        {
            if (id != cliente.ClienteId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(cliente.Identificacion))
            {
                ModelState.AddModelError("Identificacion", "La identificación es obligatoria.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Identificacion))
            {
                bool identificacionExiste = await _context.Clientes
                    .AnyAsync(c => c.Identificacion == cliente.Identificacion && c.ClienteId != cliente.ClienteId);

                if (identificacionExiste)
                {
                    ModelState.AddModelError("Identificacion", "La identificación ya está registrada.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(cliente.ClienteId))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar el cliente.");
                return View(cliente);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.ClienteId == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.ClienteId == id);
        }
    }
}
