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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("Id,NombreBotella,AlturaBotella,AnchoBotella")] BotellaDetalles botella)
        {
            if (ModelState.IsValid)
            {
                _context.Add(botella);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreBotella,AlturaBotella,AnchoBotella")] BotellaDetalles botella)
        {
            if (id != botella.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(botella);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(botella.Id))
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
            return View(botella);
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

        [Authorize(Roles = "Especialista")]
        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BotellaDetalle == null)
            {
                return Problem("Entity set 'AppDbContext.BotellaDetalle'  is null.");
            }
            var botella = await _context.BotellaDetalle.FindAsync(id);
            if (botella != null)
            {
                _context.BotellaDetalle.Remove(botella);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool ProductoExists(int id)
        {
            return (_context.BotellaDetalle?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
