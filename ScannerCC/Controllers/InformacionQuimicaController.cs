using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("Id,Cepa,MinAzucar,MaxAzucar,MinSulforoso,MaxSulfosoro,MinDensidad,MaxDensidad,MinGradoAlcohol,MaxGradoAlcohol")] InformacionQuimica infqui)
        {
            if (ModelState.IsValid)
            {
                _context.Add(infqui);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cepa,MinAzucar,MaxAzucar,MinSulforoso,MaxSulfosoro,MinDensidad,MaxDensidad,MinGradoAlcohol,MaxGradoAlcohol")] InformacionQuimica infqui)
        {
            if (id != infqui.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(infqui);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InfQuimicaExists(infqui.Id))
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
            return View(infqui);
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

        [Authorize(Roles = "Especialista")]
        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InformacionQuimica == null)
            {
                return Problem("Entity set 'AppDbContext.Producto'  is null.");
            }
            var infqui = await _context.InformacionQuimica.FindAsync(id);
            if (infqui != null)
            {
                _context.InformacionQuimica.Remove(infqui);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool InfQuimicaExists(int id)
        {
            return (_context.InformacionQuimica?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
