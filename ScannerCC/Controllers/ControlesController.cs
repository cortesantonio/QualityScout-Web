using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class ControlesController : Controller
    {
        private readonly AppDbContext _context;

        public ControlesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Controles
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Controles.Include(c => c.Productos);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Controles/Details/5
        public async Task<IActionResult> Details2(int? id)
        {
            if (id == null || _context.Controles == null)
            {
                return NotFound();
            }

            var controles = await _context.Controles
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (controles == null)
            {
                return NotFound();
            }

            return View(controles);
        }

        // GET: Controles/Create
        public IActionResult CreateControl()
        {
            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            var usuarios = _context.Usuario.Select(u => new { u.Id, u.Nombre }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre");
            ViewData["IdUsuarios"] = new SelectList(usuarios, "Id", "Nombre");
            return View();
        }

        // POST: Controles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateControl([Bind("Id,IdProductos,Linea,PaisDestino,Comentario,Tipodecontrol,Estado")] Controles controles)
        {
            // Obtener el usuario logueado
            var currentUser = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == User.Identity.Name); // Ajusta según cómo almacenas el nombre del usuario
            if (currentUser == null)
            {
                return Problem("Usuario no encontrado.");
            }

            controles.IdUsuarios = currentUser.Id; // Asignar IdUsuarios
            controles.FechaHoraPrimerControl = DateTime.Now; // Asignar fecha actual

            _context.Add(controles);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Controles/Edit/5
        public async Task<IActionResult> Edit2(int id)
        {
            var control = await _context.Controles.FindAsync(id);
            if (control == null)
            {
                return NotFound();
            }
            if (control.Estado != "Reproceso")
            {
                // Aquí puedes redirigir a otra vista o mostrar un mensaje
                return RedirectToAction(nameof(Index)); // Cambia a la acción adecuada si no está en reproceso
            }

            ViewBag.IdProductos = new SelectList(_context.Producto, "Id", "NombreProducto", control.IdProductos);
            ViewBag.IdUsuarios = new SelectList(_context.Usuario, "Id", "NombreUsuario", control.IdUsuarios);
            return View(control);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(int id, Controles model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Rellenar automáticamente con la fecha y hora actual
                    model.FechaHoraControlFinal = DateTime.Now;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ControlExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Cambia a la acción adecuada después de editar
            }
            ViewBag.IdProductos = new SelectList(_context.Producto, "Id", "NombreProducto", model.IdProductos);
            ViewBag.IdUsuarios = new SelectList(_context.Usuario, "Id", "NombreUsuario", model.IdUsuarios);
            return View(model);
        }

        // GET: Controles/Delete/5
        public async Task<IActionResult> Delete2(int? id)
        {
            if (id == null || _context.Controles == null)
            {
                return NotFound();
            }

            var controles = await _context.Controles
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (controles == null)
            {
                return NotFound();
            }

            return View(controles);
        }

		


		// POST: Controles/Delete/5
		[HttpPost]
        public IActionResult DeleteConfirmed(int id)
        { 
            var control =  _context.Controles.Where(x=>x.Id == id).FirstOrDefault();
            _context.Controles.Remove(control);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
            
        }

        private bool ControlesExists(int id)
        {
          return (_context.Controles?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool ControlExists(int id)
        {
            return _context.Controles.Any(e => e.Id == id);
        }

        [HttpGet]
		[Route("Productos/getData")]
		public IActionResult GetInfo()
		{
			var data = _context.Producto.ToList();

			return Json(data);
		}
	}


}
