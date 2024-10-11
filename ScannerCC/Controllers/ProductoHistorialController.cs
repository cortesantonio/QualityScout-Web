using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class ProductoHistorialController : Controller
    {
        private readonly AppDbContext _context;

        public ProductoHistorialController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductoHistorial == null)
            {
                return NotFound();
            }

            var productoh = await _context.ProductoHistorial
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productoh == null)
            {
                return NotFound();
            }

            return View(productoh);
        }

        // GET: Productoes/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            var productos = _context.Producto.Select(p => new { p.Id }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Id");
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("Id,IdProductos,FechaCosecha,FechaProduccion,FechaEnvasado")] ProductoHistorial productoh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productoh);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }


        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductoHistorial == null)
            {
                return NotFound();
            }

            var productoh = await _context.ProductoHistorial.FindAsync(id);
            if (productoh == null)
            {
                return NotFound();
            }

            return View(productoh);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdProductos,FechaCosecha,FechaProduccion,FechaEnvasado")] ProductoHistorial productoh)
        {
            if (id != productoh.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productoh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoHExists(productoh.Id))
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
            return View(productoh);
        }

        // GET: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductoHistorial == null)
            {
                return NotFound();
            }

            var productoh = await _context.ProductoHistorial
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productoh == null)
            {
                return NotFound();
            }

            return View(productoh);
        }

        [Authorize(Roles = "Especialista")]
        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductoHistorial == null)
            {
                return Problem("Entity set 'AppDbContext.ProductoHistorial'  is null.");
            }
            var productoh = await _context.ProductoHistorial.FindAsync(id);
            if (productoh != null)
            {
                _context.ProductoHistorial.Remove(productoh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool ProductoHExists(int id)
        {
            return (_context.ProductoHistorial?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
