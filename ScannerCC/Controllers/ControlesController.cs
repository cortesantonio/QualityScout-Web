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

        // GET: Controles/Details
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
            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre");
            return View();
        }

        // POST: Controles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateControl(int IdProductos, string Linea, string PaisDestino, string Comentario, string Tipodecontrol, string Estado)
        {
            try
            {
                // Obtener el usuario logueado
                var currentUser = await _context.Usuario.FirstOrDefaultAsync(u => u.Rut == User.Identity.Name);
                if (currentUser == null)
                {
                    return Problem("Usuario no encontrado.");
                }

                Controles control = new Controles();
                control.IdProductos = IdProductos; 
                control.Linea = Linea;
                control.PaisDestino = PaisDestino;
                control.Comentario = Comentario;
                control.Tipodecontrol = Tipodecontrol;
                control.Estado = Estado;
                control.IdUsuarios = currentUser.Id; // Asignar IdUsuarios
                control.FechaHoraPrimerControl = DateTime.Now; // Asignar fecha actual

                _context.Add(control);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el control: " + ex.Message);
            }
        }

        // GET: Controles/Edit
        public async Task<IActionResult> Edit2(int id)
        {
            try
            {
                var control = await _context.Controles
                    .Include(c => c.Productos) 
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (control == null)
                {
                    return NotFound("Control no encontrado.");
                }

                // Verificar si el estado es "Reproceso"
                if (control.Estado != "Reproceso")
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(control); // Enviar el control a la vista
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al cargar el control para edición: " + ex.Message);
            }
        }

        // POST: Controles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(int id, string Comentario, string EstadoFinal)
        {
            if (id != id) 
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Controles control = new Controles(); 
                    control = await _context.Controles.FindAsync(id);
                    if (control == null)
                    {
                        return NotFound("Control no encontrado.");
                    }

                    control.Comentario = Comentario;
                    control.EstadoFinal = EstadoFinal;
                    control.FechaHoraControlFinal = DateTime.Now; // Asignar fecha actual al finalizar el control

                    // Guardar cambios
                    _context.Update(control);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ControlExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    return Problem("Ocurrió un error al editar el control: " + ex.Message);
                }
            }

            return View();
        }


        // GET: Controles/Delete
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

        // POST: Controles/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var control = await _context.Controles.FindAsync(id);
                if (control == null)
                {
                    return NotFound("Control no encontrado.");
                }

                _context.Controles.Remove(control);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar el control: " + ex.Message);
            }
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
