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


        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
        public async Task<IActionResult> Create(string NombreBotella, int AlturaBotella, int AnchoBotella)
        {
            try
            {
                // Crear un nuevo objeto BotellaDetalles
                BotellaDetalles botella = new BotellaDetalles();
                botella.NombreBotella = NombreBotella;
                botella.AlturaBotella = AlturaBotella;
                botella.AnchoBotella = AnchoBotella;

                // Guardar en la base de datos
                _context.Add(botella);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear la botella: " + ex.Message);
            }
        }



        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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

        // POST: Productoes/Edit/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string NombreBotella, int AlturaBotella, int AnchoBotella)
        {
            try
            {
                // Obtener la botella a editar
                var botella = await _context.BotellaDetalle.FindAsync(id);
                if (botella == null)
                {
                    return NotFound("Botella no encontrada.");
                }

                // Actualizar las propiedades de la botella
                botella.NombreBotella = NombreBotella;
                botella.AlturaBotella = AlturaBotella;
                botella.AnchoBotella = AnchoBotella;

                // Guardar los cambios en la base de datos
                _context.Update(botella);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar la botella: " + ex.Message);
            }
        }


        // GET: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
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

        // POST: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Obtener la botella a eliminar
                var botella = await _context.BotellaDetalle.FindAsync(id);
                if (botella == null)
                {
                    return NotFound("Botella no encontrada.");
                }

                // Eliminar la botella de la base de datos
                _context.BotellaDetalle.Remove(botella);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
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
