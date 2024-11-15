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

        [Authorize(Roles = "Especialista")]
        public IActionResult GestionInformacion()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            ViewBag.InformacionQuimica = _context.InformacionQuimica.ToList();
            return View();
        }

        // GET: Informacion/Details
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Details(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            return View();
        }

        // POST: Informacion/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Cepa, float MinAzucar, float MaxAzucar, float MinSulfuroso, float MaxSulfuroso, float MinDensidad, float MaxDensidad, float MinGradoAlcohol, float MaxGradoAlcohol)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                if (ModelState.IsValid)
                {
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

                    _context.Add(infqui);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("GestionInformacion", "InformacionQuimica");
                }
                return RedirectToAction("GestionInformacion", "InformacionQuimica");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear la información química: " + ex.Message);
            }
        }

        // GET: Informacion/Edit
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Edit(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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

        // POST: Informacion/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Cepa, float MinAzucar, float MaxAzucar, float MinSulfuroso, float MaxSulfuroso, float MinDensidad, float MaxDensidad, float MinGradoAlcohol, float MaxGradoAlcohol)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var infqui = await _context.InformacionQuimica.FindAsync(id);
                if (infqui == null)
                {
                    return NotFound("Información química no encontrada.");
                }

                infqui.Cepa = Cepa;
                infqui.MinAzucar = MinAzucar;
                infqui.MaxAzucar = MaxAzucar;
                infqui.MinSulfuroso = MinSulfuroso;
                infqui.MaxSulfuroso = MaxSulfuroso;
                infqui.MinDensidad = MinDensidad;
                infqui.MaxDensidad = MaxDensidad;
                infqui.MinGradoAlcohol = MinGradoAlcohol;
                infqui.MaxGradoAlcohol = MaxGradoAlcohol;

                _context.Update(infqui);
                await _context.SaveChangesAsync();
                return RedirectToAction("GestionInformacion", "InformacionQuimica");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar la información química: " + ex.Message);
            }
        }

        // GET: Informacion/Delete
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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

        // POST: Informacion/Delete
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var infqui = await _context.InformacionQuimica.FindAsync(id);
                if (infqui == null)
                {
                    return NotFound("Información química no encontrada.");
                }

                _context.InformacionQuimica.Remove(infqui);
                await _context.SaveChangesAsync();
                return RedirectToAction("GestionInformacion", "InformacionQuimica");
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
