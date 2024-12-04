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

        [Authorize(Roles = "Especialista")]
        public IActionResult GestionBotellas() // se carga la vista prinpal de botellas, el listado con las opciones.
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            ViewBag.BotellaDetalles = _context.BotellaDetalle.ToList();
            return View();
        }

        // GET: Botellas/Details
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Details(int? id) // se carga una botella en especifico
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo; 

            if (id == null || _context.BotellaDetalle == null)
            {
                return NotFound();
            }
            // se obtienen las botellas de manera asincrona, espera a obtener el resultado.
            var botella = await _context.BotellaDetalle 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (botella == null)
            {
                return NotFound();
            }

            return View(botella);
        }

        // GET: Botellas/Create - pagina para crear registros
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
            // recibe la info y por medio de try-catch se asegura de controlar los errores.
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

        // GET: Botellas/Edit - vista para editar la info de botella.
        [Authorize(Roles = "Especialista")]
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

        // POST: Botellas/Edit  - metodo para editar la info recibida de la vista edit. (metodo de arriba)
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string NombreBotella, int AlturaBotella, int AnchoBotella)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            // recibe la info y por medio de try-catch se asegura de controlar los errores.
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


        // GET: Botellas/Delete - vista para eliminar el item.
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;
            // se asegura que el id recibido sea diferente a nulo y que hayan registros en la tabla de botellas.
            if (id == null || _context.BotellaDetalle == null)
            {
                return NotFound();
            }
            // obtiene el item y lo pasa a la vista 
            var botella = await _context.BotellaDetalle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (botella == null)
            {
                return NotFound();
            }

            return View(botella);
        }

        // POST: Botellas/Delete - metodo eliminador del item
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;
            // try catch para controlar posibles errores
            try
            {    //buscar el item 
                var botella = await _context.BotellaDetalle.FindAsync(id);
                if (botella == null)
                {
                    return NotFound("Botella no encontrada.");
                }
                // elimina el item pasando el resultado de la busqueda anterior
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
