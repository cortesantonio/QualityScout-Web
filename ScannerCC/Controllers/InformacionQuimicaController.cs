using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;
using System.Globalization;

namespace ScannerCC.Controllers
{
    public class InformacionQuimicaController : Controller
    {
        private readonly AppDbContext _context;

        public InformacionQuimicaController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InformacionQuimica == null)
            {
                return NotFound();
            }

            var infqui = await _context.InformacionQuimica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (infqui == null)
            {
                return NotFound();
            }

            return View(infqui);
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
        public async Task<IActionResult> Create(string Cepa, float MinAzucar, float MaxAzucar, float MinSulfuroso, float MaxSulfuroso, float MinDensidad, float MaxDensidad, float MinGradoAlcohol, float MaxGradoAlcohol)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Crear nueva instancia de InformacionQuimica
                    InformacionQuimica infqui = new InformacionQuimica();
                    infqui.Cepa = Cepa;
                    infqui.MinAzucar = MinAzucar;
                    infqui.MaxAzucar = MaxAzucar;
                    infqui.MinSulfuroso = MinSulfuroso;
                    infqui.MaxSulfuroso = MaxSulfuroso;
                    infqui.MinDensidad = MinDensidad;
                    infqui.MaxDensidad = MaxDensidad;
                    infqui.MinGradoAlcohol = MinGradoAlcohol;
                    infqui.MaxGradoAlcohol = MaxGradoAlcohol;

                    // Agregar a la base de datos
                    _context.Add(infqui);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }

                // En caso de que el modelo no sea válido, redirigir
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear la información química: " + ex.Message);
            }
        }

        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InformacionQuimica == null)
            {
                return NotFound();
            }

            var infqui = await _context.InformacionQuimica.FindAsync(id);
            if (infqui == null)
            {
                return NotFound();
            }
            return View(infqui);
        }

        // POST: Productoes/Edit/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Cepa, float MinAzucar, float MaxAzucar, float MinSulfuroso, float MaxSulfuroso, float MinDensidad, float MaxDensidad, float MinGradoAlcohol, float MaxGradoAlcohol)
        {
            try
            {
                // Obtener el producto a editar
                var infqui = await _context.InformacionQuimica.FindAsync(id);
                if (infqui == null)
                {
                    return NotFound("Información química no encontrada.");
                }

                // Actualizar propiedades
                infqui.Cepa = Cepa;
                infqui.MinAzucar = MinAzucar;
                infqui.MaxAzucar = MaxAzucar;
                infqui.MinSulfuroso = MinSulfuroso;
                infqui.MaxSulfuroso = MaxSulfuroso;
                infqui.MinDensidad = MinDensidad;
                infqui.MaxDensidad = MaxDensidad;
                infqui.MinGradoAlcohol = MinGradoAlcohol;
                infqui.MaxGradoAlcohol = MaxGradoAlcohol;

                // Guardar cambios
                _context.Update(infqui);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar la información química: " + ex.Message);
            }
        }


        // GET: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InformacionQuimica == null)
            {
                return NotFound();
            }

            var infqui = await _context.InformacionQuimica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (infqui == null)
            {
                return NotFound();
            }

            return View(infqui);
        }

        // POST: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Obtener la información química a eliminar
                var infqui = await _context.InformacionQuimica.FindAsync(id);
                if (infqui == null)
                {
                    return NotFound("Información química no encontrada.");
                }

                // Eliminar de la base de datos
                _context.InformacionQuimica.Remove(infqui);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar la información química: " + ex.Message);
            }
        }


        private bool InfQuimicaExists(int id)
        {
            return (_context.InformacionQuimica?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
