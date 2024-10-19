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


        // GET: Productoes/Details/5
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

        // GET: Productoes/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productoes/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Titulo, string Enfoque, string Descripcion)
        {
            try
            {
                // Obtener el usuario actualmente logueado
                var currentUser = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == User.Identity.Name); // Ajusta según cómo almacenas el nombre del usuario
                if (currentUser == null)
                {
                    return Problem("Usuario no encontrado.");
                }

                // Crear un nuevo informe
                Informes informes = new Informes();
                informes.IdUsuarios = currentUser.Id; // Asigna el Id del usuario logueado
                informes.Titulo = Titulo;
                informes.Enfoque = Enfoque;
                informes.Fecha = DateTime.Now; // Asigna la fecha actual
                informes.Descripcion = Descripcion;

                // Guardar en la base de datos
                _context.Add(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el informe: " + ex.Message);
            }
        }



        // GET: Productoes/Edit/5
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

        // POST: Productoes/Edit/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Titulo, string Enfoque, string Descripcion)
        {
            try
            {
                // Obtener el informe a editar
                var informes = await _context.Informe.FindAsync(id);
                if (informes == null)
                {
                    return NotFound("Informe no encontrado.");
                }

                // Asignar las propiedades del informe
                informes.Titulo = Titulo;
                informes.Enfoque = Enfoque;
                informes.Descripcion = Descripcion;
                // No es necesario actualizar la fecha, a menos que desees cambiarla

                // Guardar cambios
                _context.Update(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar el informe: " + ex.Message);
            }
        }


        // GET: Productoes/Delete/5
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

        // POST: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Obtener el informe a eliminar
                var informes = await _context.Informe.FindAsync(id);
                if (informes == null)
                {
                    return NotFound("Informe no encontrado.");
                }

                // Eliminar el informe
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
