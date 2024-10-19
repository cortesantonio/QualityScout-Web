using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;
using System.ComponentModel.DataAnnotations.Schema;

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
            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            var botellaDetalles = _context.BotellaDetalle.Select(bd => new { bd.Id, bd.NombreBotella }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre");
            ViewData["IdBotellaDetalles"] = new SelectList(botellaDetalles, "Id", "NombreBotella");

            return View();
        }

        // POST: Productoes/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int IdProductos, int IdBotellaDetalles, int Capacidad, string TipoCapsula, string TipoEtiqueta, string ColorBotella, bool Medalla, string ColorCapsula,
                                                string TipoCorcho, int MedidaEtiquetaABoquete, int MedidaEtiquetaABase)
        {
            try
            {
                ProductoDetalles productoDetalle = new ProductoDetalles();
                productoDetalle.IdProductos = IdProductos;
                productoDetalle.IdBotellaDetalles = IdBotellaDetalles;
                productoDetalle.Capacidad = Capacidad;
                productoDetalle.TipoCapsula = TipoCapsula;
                productoDetalle.TipoEtiqueta = TipoEtiqueta;
                productoDetalle.ColorBotella = ColorBotella;
                productoDetalle.Medalla = Medalla;
                productoDetalle.ColorCapsula = ColorCapsula;
                productoDetalle.TipoCorcho = TipoCorcho;
                productoDetalle.MedidaEtiquetaABoquete = MedidaEtiquetaABoquete;
                productoDetalle.MedidaEtiquetaABase = MedidaEtiquetaABase;

                _context.ProductoDetalle.Add(productoDetalle);
                await _context.SaveChangesAsync(); 
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el detalle del producto: " + ex.Message);
            }
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
            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            var botellaDetalles = _context.BotellaDetalle.Select(bd => new { bd.Id, bd.NombreBotella }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre", productod.IdProductos);
            ViewData["IdBotellaDetalles"] = new SelectList(botellaDetalles, "Id", "NombreBotella", productod.IdBotellaDetalles);
            return View(productod);
        }

        // POST: Productoes/Edit/
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int IdProductos, int IdBotellaDetalles, int Capacidad, string TipoCapsula, string TipoEtiqueta, string ColorBotella, bool Medalla, string ColorCapsula,
                                        string TipoCorcho, int MedidaEtiquetaABoquete, int MedidaEtiquetaABase)
        {
            try
            {
                // Obtener el producto detalle a editar
                var productoDetalle = await _context.ProductoDetalle.FindAsync(id);
                if (productoDetalle == null)
                {
                    return NotFound("Detalle del producto no encontrado.");
                }

                // Actualizar propiedades del producto detalle
                productoDetalle.IdProductos = IdProductos;
                productoDetalle.IdBotellaDetalles = IdBotellaDetalles;
                productoDetalle.Capacidad = Capacidad;
                productoDetalle.TipoCapsula = TipoCapsula;
                productoDetalle.TipoEtiqueta = TipoEtiqueta;
                productoDetalle.ColorBotella = ColorBotella;
                productoDetalle.Medalla = Medalla;
                productoDetalle.ColorCapsula = ColorCapsula;
                productoDetalle.TipoCorcho = TipoCorcho;
                productoDetalle.MedidaEtiquetaABoquete = MedidaEtiquetaABoquete;
                productoDetalle.MedidaEtiquetaABase = MedidaEtiquetaABase;

                // Guardar cambios
                _context.ProductoDetalle.Update(productoDetalle);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar el detalle del producto: " + ex.Message);
            }
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


        // POST: Productoes/Delete/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Obtener el producto detalle a eliminar
                var productoDetalle = await _context.ProductoDetalle.FindAsync(id);
                if (productoDetalle == null)
                {
                    return NotFound("Detalle del producto no encontrado.");
                }

                // Eliminar el producto detalle
                _context.ProductoDetalle.Remove(productoDetalle);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar el detalle del producto: " + ex.Message);
            }
        }

        private bool ProductoDExists(int id)
        {
            return (_context.ProductoDetalle?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
