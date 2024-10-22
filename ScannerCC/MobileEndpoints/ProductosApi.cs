using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace QualityScout.MobileEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosApi(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductosApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Productos>>> GetProducto()
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            return await _context.Producto.Include(x =>x.InformacionQuimica).ToListAsync();
        }

        // GET: api/ProductosApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Productos>> GetProducto(int id)
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            var producto = await _context.Producto
                 .Include(p => p.InformacionQuimica)
                 .Include(p => p.ProductoDetalles)
                     .ThenInclude(pd => pd.BotellaDetalles)
                 .Include(p => p.ProductoHistorial)
                 .Include(p => p.Usuarios)
                 .FirstOrDefaultAsync(m => m.Id == id);


            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // PUT: api/ProductosApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateActivo/{id}")]
        public async Task<IActionResult> UpdateActivo(int id)
        {
            var p = _context.Producto.Where(x => x.Id == id).FirstOrDefault();

            if (p == null)
            {
                return NotFound();
            }

            if (p.Activo == true)
            {
                p.Activo = false;
            }
            else
            {
                p.Activo = true;
            }

            _context.Producto.Update(p);
            _context.SaveChanges();
            return Ok();


        }

        // POST: api/ProductosApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Productos>> PostProducto(Productos producto)
        {
            if (_context.Producto == null)
            {
                return Problem("Entity set 'AppDbContext.Producto'  is null.");
            }
            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
        }

        // DELETE: api/ProductosApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return (_context.Producto?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
