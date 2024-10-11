using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class EscaneosController : Controller
    {
        private readonly AppDbContext _context;

        public EscaneosController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Escaneo == null)
            {
                return NotFound();
            }

            var escaneo = await _context.Escaneo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (escaneo == null)
            {
                return NotFound();
            }

            return View(escaneo);
        }

        // GET: Productoes/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            var productos = _context.Producto.Select(p => new { p.Id }).ToList();
            var usuarios = _context.Usuario.Select(u => new { u.Id }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Id");
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
    [Bind("Id,IdProductos,IdUsuarios,Fecha,Hora")] Escaneos escaneo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(escaneo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }


        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Escaneo == null)
            {
                return NotFound();
            }

            var escaneo = await _context.Escaneo.FindAsync(id);
            if (escaneo == null)
            {
                return NotFound();
            }
            var productos = _context.Producto.Select(p => new { p.Id }).ToList();
            var usuarios = _context.Usuario.Select(u => new { u.Id }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Id");
            ViewData["IdUsuarios"] = new SelectList(usuarios, "Id", "Id");
            return View(escaneo);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdProductos,IdUsuarios,Fecha,Hora")] Escaneos escaneo)
        {
            if (id != escaneo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(escaneo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EscaneoExists(escaneo.Id))
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
            return View(escaneo);
        }

        // GET: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Escaneo == null)
            {
                return NotFound();
            }

            var escaneo = await _context.Escaneo
                .FirstOrDefaultAsync(m => m.Id == id);
            if ( escaneo == null)
            {
                return NotFound();
            }

            return View(escaneo);
        }

        [Authorize(Roles = "Especialista")]
        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Producto == null)
            {
                return Problem("Entity set 'AppDbContext.BotellaDetalle'  is null.");
            }
            var escaneo = await _context.BotellaDetalle.FindAsync(id);
            if (escaneo != null)
            {
                _context.BotellaDetalle.Remove(escaneo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool EscaneoExists(int id)
        {
            return (_context.BotellaDetalle?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
