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
    public class ReservasController : Controller
    {
        private readonly RestauranteDalyContext _context;

        public ReservasController(RestauranteDalyContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var restauranteDalyContext = _context.Reservas.Include(r => r.Cliente).Include(r => r.Mesa);
            return View(await restauranteDalyContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.ReservaId == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre");
            ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "MesaId");
            ViewData["Estado"] = new SelectList(new List<string>
            {
                "Pendiente",
                "Confirmada",
                "Cancelada"
            });
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaId,ClienteId,MesaId,Fecha,CantidadPersonas,Estado")] Reserva reserva)
        {
            if (reserva.Fecha == null)
            {
                ModelState.AddModelError("Fecha", "La fecha de la reserva es obligatoria.");
            }
            else if (reserva.Fecha.Value < DateTime.Now)
            {
                ModelState.AddModelError("Fecha", "No se puede reservar en una fecha pasada.");
            }

            if (reserva.MesaId == null)
            {
                ModelState.AddModelError("MesaId", "Debe seleccionar una mesa.");
            }

            if (reserva.CantidadPersonas == null || reserva.CantidadPersonas <= 0)
            {
                ModelState.AddModelError("CantidadPersonas", "La cantidad de personas debe ser mayor que cero.");
            }

            if (reserva.MesaId != null && reserva.CantidadPersonas != null && reserva.CantidadPersonas > 0)
            {
                var mesa = await _context.Mesas.FindAsync(reserva.MesaId);

                if (mesa == null)
                {
                    ModelState.AddModelError("MesaId", "La mesa seleccionada no existe.");
                }
                else if (mesa.Capacidad != null && reserva.CantidadPersonas > mesa.Capacidad)
                {
                    ModelState.AddModelError("CantidadPersonas",
                        $"La mesa seleccionada tiene capacidad para {mesa.Capacidad} personas.");
                }
            }

            if (reserva.MesaId != null && reserva.Fecha != null)
            {
                bool reservaDuplicada = await _context.Reservas
                    .AnyAsync(r => r.MesaId == reserva.MesaId && r.Fecha == reserva.Fecha);

                if (reservaDuplicada)
                {
                    ModelState.AddModelError("", "Ya existe una reserva para esa mesa en la misma fecha y hora.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", reserva.ClienteId);
                ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "MesaId", reserva.MesaId);
                ViewData["Estado"] = new SelectList(new List<string>
                {
                    "Pendiente",
                    "Confirmada",
                    "Cancelada"
                }, reserva.Estado);
                return View(reserva);
            }

            try
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar la reserva.");
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", reserva.ClienteId);
                ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "MesaId", reserva.MesaId);
                ViewData["Estado"] = new SelectList(new List<string>
                {
                    "Pendiente",
                    "Confirmada",
                    "Cancelada"
                }, reserva.Estado);
                return View(reserva);
            }
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", reserva.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "MesaId", reserva.MesaId);
            ViewData["Estado"] = new SelectList(new List<string>
            {
                "Pendiente",
                "Confirmada",
                "Cancelada"
            }, reserva.Estado);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservaId,ClienteId,MesaId,Fecha,CantidadPersonas,Estado")] Reserva reserva)
        {
            if (id != reserva.ReservaId)
            {
                return NotFound();
            }

            if (reserva.Fecha == null)
            {
                ModelState.AddModelError("Fecha", "La fecha de la reserva es obligatoria.");
            }
            else if (reserva.Fecha.Value < DateTime.Now)
            {
                ModelState.AddModelError("Fecha", "No se puede reservar en una fecha pasada.");
            }

            if (reserva.MesaId == null)
            {
                ModelState.AddModelError("MesaId", "Debe seleccionar una mesa.");
            }

            if (reserva.CantidadPersonas == null || reserva.CantidadPersonas <= 0)
            {
                ModelState.AddModelError("CantidadPersonas", "La cantidad de personas debe ser mayor que cero.");
            }

            if (reserva.MesaId != null && reserva.CantidadPersonas != null && reserva.CantidadPersonas > 0)
            {
                var mesa = await _context.Mesas.FindAsync(reserva.MesaId);

                if (mesa == null)
                {
                    ModelState.AddModelError("MesaId", "La mesa seleccionada no existe.");
                }
                else if (mesa.Capacidad != null && reserva.CantidadPersonas > mesa.Capacidad)
                {
                    ModelState.AddModelError("CantidadPersonas",
                        $"La mesa seleccionada tiene capacidad para {mesa.Capacidad} personas.");
                }
            }

            if (reserva.MesaId != null && reserva.Fecha != null)
            {
                bool reservaDuplicada = await _context.Reservas
                    .AnyAsync(r => r.MesaId == reserva.MesaId
                                && r.Fecha == reserva.Fecha
                                && r.ReservaId != reserva.ReservaId);

                if (reservaDuplicada)
                {
                    ModelState.AddModelError("", "Ya existe una reserva para esa mesa en la misma fecha y hora.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", reserva.ClienteId);
                ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "MesaId", reserva.MesaId);
                ViewData["Estado"] = new SelectList(new List<string>
                {
                    "Pendiente",
                    "Confirmada",
                    "Cancelada"
                }, reserva.Estado);
                return View(reserva);
            }

            try
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(reserva.ReservaId))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar la reserva.");
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", reserva.ClienteId);
                ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "MesaId", reserva.MesaId);
                ViewData["Estado"] = new SelectList(new List<string>
                {
                    "Pendiente",
                    "Confirmada",
                    "Cancelada"
                }, reserva.Estado);
                return View(reserva);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.ReservaId == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaId == id);
        }
    }
}
