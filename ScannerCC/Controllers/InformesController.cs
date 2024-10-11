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
            var usuarios = _context.BotellaDetalle.Select(bd => new { bd.Id }).ToList();
            ViewData["IdUsuarios"] = new SelectList(usuarios, "Id", "Id");
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("Id,IdUsuarios,Titulo,Enfoque,Fecha,Descripcion")] Informes informes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(informes);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
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
            var usuarios = _context.BotellaDetalle.Select(bd => new { bd.Id }).ToList();
            ViewData["IdUsuarios"] = new SelectList(usuarios, "Id", "Id");
            return View(informes);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUsuarios,Titulo,Enfoque,Fecha,Descripcion")] Informes informes)
        {
            if (id != informes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(informes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InfExists(informes.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(informes);
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

        [Authorize(Roles = "Especialista")]
        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Informe == null)
            {
                return Problem("Entity set 'AppDbContext.Informe'  is null.");
            }
            var informes = await _context.Informe.FindAsync(id);
            if (informes != null)
            {
                _context.Informe.Remove(informes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool InfExists(int id)
        {
            return (_context.Informe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
