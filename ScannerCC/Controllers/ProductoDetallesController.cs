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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int IdProductos, int IdBotellaDetalles , int Capacidad ,  string TipoCapsula , string TipoEtiqueta , string ColorBotella, bool Medalla, string ColorCapsula,
                                                    string TipoCorcho , int MedidaEtiquetaABoquete , int MedidaEtiquetaABase)
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
                _context.SaveChanges();
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
            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();
            var botellaDetalles = _context.BotellaDetalle.Select(bd => new { bd.Id, bd.NombreBotella }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Nombre", productod.IdProductos);
            ViewData["IdBotellaDetalles"] = new SelectList(botellaDetalles, "Id", "NombreBotella", productod.IdBotellaDetalles);
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
