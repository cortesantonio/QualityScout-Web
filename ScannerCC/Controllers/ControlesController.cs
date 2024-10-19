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
            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre");
            return View();
        }

        // POST: Controles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateControl(string IdProductos, string Linea, string PaisDestino, string Comentario, string Tipodecontrol, string Estado)
        {
            try
            {
                // Obtener el usuario logueado
                var currentUser = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (currentUser == null)
                {
                    return Problem("Usuario no encontrado.");
                }

                // Crear y asignar el nuevo control
                Controles control = new Controles(); // Usando new Controles() en lugar de inicialización
                control.IdProductos = int.Parse(IdProductos); // Asignar IdProductos
                control.Linea = Linea;
                control.PaisDestino = PaisDestino;
                control.Comentario = Comentario;
                control.Tipodecontrol = Tipodecontrol;
                control.Estado = Estado;
                control.IdUsuarios = currentUser.Id; // Asignar IdUsuarios
                control.FechaHoraPrimerControl = DateTime.Now; // Asignar fecha actual

                // Guardar control
                _context.Add(control);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el control: " + ex.Message);
            }
        }

        // GET: Controles/Edit/5
        public async Task<IActionResult> Edit2(int id)
        {
            try
            {
                var control = await _context.Controles.FindAsync(id);
                if (control == null)
                {
                    return NotFound("Control no encontrado.");
                }

                // Verificar si el estado es "Reproceso"
                if (control.Estado != "Reproceso")
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.IdProductos = new SelectList(_context.Producto, "Id", "Nombre", control.IdProductos);
                return View(control);
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al cargar el control para edición: " + ex.Message);
            }
        }

        // POST: Controles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(int id, string IdProductos, string Linea, string PaisDestino, string Comentario, string Tipodecontrol, string EstadoFinal)
        {
            if (id != id) // Se mantiene como estaba para verificar que el id coincida
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el control a editar
                    Controles control = new Controles(); // Usando new Controles() en lugar de inicialización
                    control = await _context.Controles.FindAsync(id);
                    if (control == null)
                    {
                        return NotFound("Control no encontrado.");
                    }

                    // Asignar propiedades para editar
                    control.IdProductos = int.Parse(IdProductos); // Asignar IdProductos
                    control.Linea = Linea;
                    control.PaisDestino = PaisDestino;
                    control.Comentario = Comentario;
                    control.Tipodecontrol = Tipodecontrol;
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

            ViewBag.IdProductos = new SelectList(_context.Producto, "Id", "Nombre", IdProductos);
            return View();
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
