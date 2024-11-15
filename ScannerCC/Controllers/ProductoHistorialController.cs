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

        [Authorize(Roles = "Especialista")]
        public IActionResult GestionProductosH()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            ViewBag.ProductoHistorial = _context.ProductoHistorial.ToList();
            return View();
        }

        [Authorize(Roles = "Especialista")]
        // GET: ProductoH/Details
        public async Task<IActionResult> Details(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            var productosConHistorial = _context.ProductoHistorial
            .Select(pd => pd.IdProductos)
            .ToList(); 

            var productosDisponibles = _context.Producto
                .Where(p => !productosConHistorial.Contains(p.Id)) 
                .Select(p => new { p.Id, p.Nombre })
                .ToList();

            ViewData["IdProductos"] = new SelectList(productosDisponibles, "Id", "Nombre");
            return View();
        }

        // POST: ProductoH/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int IdProductos, DateTime FechaCosecha, DateTime FechaProduccion, DateTime FechaEnvasado)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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
                    return RedirectToAction("GestionProductosH", "ProductoHistorial");
                }
                return RedirectToAction("GestionProductosH", "ProductoHistorial");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el historial del producto: " + ex.Message);
            }
        }

        // GET: ProductoH/Edit
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Edit(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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
                    return RedirectToAction("GestionProductosH", "ProductoHistorial");
                }
                return RedirectToAction("GestionProductosH", "ProductoHistorial");
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
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var productoh = await _context.ProductoHistorial.FindAsync(id);
                if (productoh == null)
                {
                    return NotFound("Historial del producto no encontrado.");
                }

                _context.ProductoHistorial.Remove(productoh);
                await _context.SaveChangesAsync();

                return RedirectToAction("GestionProductosH", "ProductoHistorial");
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
