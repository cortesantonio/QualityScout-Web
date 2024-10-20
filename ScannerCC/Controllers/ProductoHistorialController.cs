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

        // GET: ProductoH/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductoHistorial == null)
            {
                return NotFound();
            }

            var productoh = await _context.ProductoHistorial
                .Include(e => e.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productoh == null)
            {
                return NotFound();
            }

            return View(productoh);
        }

        // GET: ProductoH/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre");
            return View();
        }

        // POST: ProductoH/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int IdProductos, DateTime FechaCosecha, DateTime FechaProduccion, DateTime FechaEnvasado)
        {
            try
            {
                ProductoHistorial productoh = new ProductoHistorial();
                productoh.IdProductos = IdProductos;
                productoh.FechaCosecha = FechaCosecha;
                productoh.FechaProduccion = FechaProduccion;
                productoh.FechaEnvasado = FechaEnvasado;

                if (ModelState.IsValid)
                {
                    _context.Add(productoh);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el historial del producto: " + ex.Message);
            }
        }

        // GET: ProductoH/Edit
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

            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre", productoh.IdProductos);

            return View(productoh);
        }

        // POST: ProductoH/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int IdProductos, DateTime FechaCosecha, DateTime FechaProduccion, DateTime FechaEnvasado)
        {
            try
            {
                var productoh = await _context.ProductoHistorial.FindAsync(id);
                if (productoh == null)
                {
                    return NotFound("Historial del producto no encontrado.");
                }
                productoh.IdProductos = IdProductos;
                productoh.FechaCosecha = FechaCosecha;
                productoh.FechaProduccion = FechaProduccion;
                productoh.FechaEnvasado = FechaEnvasado;

                if (ModelState.IsValid)
                {
                    _context.Update(productoh);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar el historial del producto: " + ex.Message);
            }
        }

        // GET: ProductoH/Delete
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

        // POST: ProductoH/Delete
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var productoh = await _context.ProductoHistorial.FindAsync(id);
                if (productoh == null)
                {
                    return NotFound("Historial del producto no encontrado.");
                }

                _context.ProductoHistorial.Remove(productoh);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar el historial del producto: " + ex.Message);
            }
        }

        private bool ProductoHExists(int id)
        {
            return (_context.ProductoHistorial?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
