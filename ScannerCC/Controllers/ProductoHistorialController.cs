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
            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre");
            return View();
        }

        // POST: Productoes/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateTime FechaCosecha, DateTime FechaProduccion, DateTime FechaEnvasado)
        {
            try
            {
                // Crear un nuevo objeto de ProductoHistorial
                ProductoHistorial productoh = new ProductoHistorial();
                productoh.FechaCosecha = FechaCosecha;
                productoh.FechaProduccion = FechaProduccion;
                productoh.FechaEnvasado = FechaEnvasado;

                // Validar el modelo
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

            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre", productoh.IdProductos);

            return View(productoh);
        }

        // POST: Productoes/Edit/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime FechaCosecha, DateTime FechaProduccion, DateTime FechaEnvasado)
        {
            try
            {
                // Obtener el producto historial a editar
                var productoh = await _context.ProductoHistorial.FindAsync(id);
                if (productoh == null)
                {
                    return NotFound("Historial del producto no encontrado.");
                }

                // Asignar propiedades del producto historial
                productoh.FechaCosecha = FechaCosecha;
                productoh.FechaProduccion = FechaProduccion;
                productoh.FechaEnvasado = FechaEnvasado;

                // Validar el modelo
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Obtener el producto historial a eliminar
                var productoh = await _context.ProductoHistorial.FindAsync(id);
                if (productoh == null)
                {
                    return NotFound("Historial del producto no encontrado.");
                }

                // Eliminar el producto historial
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
