using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class ProductoDetallesController : Controller
    {
        private readonly AppDbContext _context;

        public ProductoDetallesController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductoDetalle == null)
            {
                return NotFound();
            }

            var productod = await _context.ProductoDetalle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productod == null)
            {
                return NotFound();
            }

            return View(productod);
        }

        // GET: Productoes/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            var productos = _context.Producto.Select(p => new { p.Id }).ToList();
            var botellaDetalles = _context.BotellaDetalle.Select(bd => new { bd.Id }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Id");
            ViewData["IdBotellaDetalles"] = new SelectList(botellaDetalles, "Id", "Id");

            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("Id,IdProductos,IdBotellaDetalles,Capacidad,TipoCapsula,TipoEtiqueta,ColorBotella,Medalla," +
    "ColorCapsula,TipoCorcho,TipoBotella,MedidaEtiquetaABoquete,MedidaEtiquetaABase")] ProductoDetalles productod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productod);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }


        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductoDetalle == null)
            {
                return NotFound();
            }

            var productod = await _context.ProductoDetalle.FindAsync(id);
            if (productod == null)
            {
                return NotFound();
            }
            var productos = _context.Producto.Select(p => new { p.Id }).ToList();
            var botellaDetalles = _context.BotellaDetalle.Select(bd => new { bd.Id }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Id");
            ViewData["IdBotellaDetalles"] = new SelectList(botellaDetalles, "Id", "Id");
            return View(productod);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdProductos,IdBotellaDetalles,Capacidad,TipoCapsula,TipoEtiqueta,ColorBotella,Medalla,ColorCapsula,TipoCorcho,TipoBotella,MedidaEtiquetaABoquete,MedidaEtiquetaABase")] ProductoDetalles productod)
        {
            if (id != productod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoDExists(productod.Id))
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
            return View(productod);
        }

        // GET: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductoDetalle == null)
            {
                return NotFound();
            }

            var productod = await _context.ProductoDetalle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productod == null)
            {
                return NotFound();
            }

            return View(productod);
        }

        [Authorize(Roles = "Especialista")]
        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductoDetalle == null)
            {
                return Problem("Entity set 'AppDbContext.ProductoDetalle'  is null.");
            }
            var productod = await _context.ProductoDetalle.FindAsync(id);
            if (productod != null)
            {
                _context.ProductoDetalle.Remove(productod);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool ProductoDExists(int id)
        {
            return (_context.ProductoDetalle?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
