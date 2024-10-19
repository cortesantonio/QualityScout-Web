using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class InformesController : Controller
    {
        private readonly AppDbContext _context;

        public InformesController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Informes/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Informe == null)
            {
                return NotFound();
            }

            var informes = await _context.Informe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (informes == null)
            {
                return NotFound();
            }

            return View(informes);
        }

        // GET: Informes/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Informes/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Titulo, string Enfoque, string Descripcion)
        {
            try
            {
                var currentUser = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == User.Identity.Name); 
                if (currentUser == null)
                {
                    return Problem("Usuario no encontrado.");
                }

                Informes informes = new Informes();
                informes.IdUsuarios = currentUser.Id; // Asigna el Id del usuario logueado
                informes.Titulo = Titulo;
                informes.Enfoque = Enfoque;
                informes.Fecha = DateTime.Now; // Asigna la fecha actual
                informes.Descripcion = Descripcion;

                _context.Add(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el informe: " + ex.Message);
            }
        }

        // GET: Informes/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Informe == null)
            {
                return NotFound();
            }

            var informes = await _context.Informe.FindAsync(id);
            if (informes == null)
            {
                return NotFound();
            }
            return View(informes);
        }

        // POST: Informes/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Titulo, string Enfoque, string Descripcion)
        {
            try
            {
                var informes = await _context.Informe.FindAsync(id);
                if (informes == null)
                {
                    return NotFound("Informe no encontrado.");
                }

                informes.Titulo = Titulo;
                informes.Enfoque = Enfoque;
                informes.Descripcion = Descripcion;

                _context.Update(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar el informe: " + ex.Message);
            }
        }


        // GET: Informes/Delete
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Informe == null)
            {
                return NotFound();
            }

            var informes = await _context.Informe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (informes == null)
            {
                return NotFound();
            }

            return View(informes);
        }

        // POST: Informes/Delete
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var informes = await _context.Informe.FindAsync(id);
                if (informes == null)
                {
                    return NotFound("Informe no encontrado.");
                }

                _context.Informe.Remove(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar el informe: " + ex.Message);
            }
        }

        private bool InfExists(int id)
        {
            return (_context.Informe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
