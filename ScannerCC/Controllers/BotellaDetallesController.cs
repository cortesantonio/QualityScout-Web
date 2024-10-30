using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class BotellaDetallesController : Controller
    {
        private readonly AppDbContext _context;

        public BotellaDetallesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult GestionBotellas()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            ViewBag.BotellaDetalles = _context.BotellaDetalle.ToList();
            return View();
        }

        // GET: Botellas/Details
        public async Task<IActionResult> Details(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo; 

            if (id == null || _context.BotellaDetalle == null)
            {
                return NotFound();
            }

            var botella = await _context.BotellaDetalle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (botella == null)
            {
                return NotFound();
            }

            return View(botella);
        }

        // GET: Botellas/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;
            return View();
        }

        // POST: Botellas/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string NombreBotella, int AlturaBotella, int AnchoBotella)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                BotellaDetalles botella = new BotellaDetalles();
                botella.NombreBotella = NombreBotella;
                botella.AlturaBotella = AlturaBotella;
                botella.AnchoBotella = AnchoBotella;

                _context.Add(botella);
                await _context.SaveChangesAsync();

                return RedirectToAction("GestionBotellas", "BotellaDetalles");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear la botella: " + ex.Message);
            }
        }

        // GET: Botellas/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo; 

            if (id == null || _context.BotellaDetalle == null)
            {
                return NotFound();
            }

            var botella = await _context.BotellaDetalle.FindAsync(id);
            if (botella == null)
            {
                return NotFound();
            }
            return View(botella);
        }

        // POST: Botellas/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string NombreBotella, int AlturaBotella, int AnchoBotella)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var botella = await _context.BotellaDetalle.FindAsync(id);
                if (botella == null)
                {
                    return NotFound("Botella no encontrada.");
                }

                botella.NombreBotella = NombreBotella;
                botella.AlturaBotella = AlturaBotella;
                botella.AnchoBotella = AnchoBotella;

                _context.Update(botella);
                await _context.SaveChangesAsync();

                return RedirectToAction("GestionBotellas", "BotellaDetalles");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar la botella: " + ex.Message);
            }
        }


        // GET: Botellas/Delete
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.BotellaDetalle == null)
            {
                return NotFound();
            }

            var botella = await _context.BotellaDetalle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (botella == null)
            {
                return NotFound();
            }

            return View(botella);
        }

        // POST: Botellas/Delete
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var botella = await _context.BotellaDetalle.FindAsync(id);
                if (botella == null)
                {
                    return NotFound("Botella no encontrada.");
                }

                _context.BotellaDetalle.Remove(botella);
                await _context.SaveChangesAsync();

                return RedirectToAction("GestionBotellas", "BotellaDetalles");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar la botella: " + ex.Message);
            }
        }

        private bool ProductoExists(int id)
        {
            return (_context.BotellaDetalle?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
