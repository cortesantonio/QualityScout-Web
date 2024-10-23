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
    // DTO para recibir los datos del control
    public class ControlRecibido
    {
        public int IdProducto { get; set; }
        public string Linea { get; set; }
        public string PaisDestino { get; set; }
        public string Comentario { get; set; }
        public string TipoDeControl { get; set; }
        public string Estado { get; set; }
        public int IdUsuario { get; set; }
    }


    public class ControlUpdate
    {
        public int id { get; set; }
        public string comentario { get; set; }
        public string estadoFinal { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ApiControles : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiControles(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiControles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Controles>>> GetControles()
        {
            if (_context.Controles == null)
            {
                return NotFound("El recurso 'Controles' no se encuentra disponible.");
            }

            return await _context.Controles.Include(x =>x.Productos).Include(x=>x.Usuarios).ToListAsync();
        }

        // GET: api/ApiControles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Controles>> GetControles(int id)
        {
            if (_context.Controles == null)
            {
                return NotFound("El recurso 'Controles' no se encuentra disponible.");
            }

            var controles = await _context.Controles.FindAsync(id);

            if (controles == null)
            {
                return NotFound($"Control con id {id} no encontrado.");
            }

            return controles;
        }


        // POST: api/ApiControles/CrearControl
        [HttpPost("CrearControl")]
        public async Task<ActionResult<Controles>> PostControles(ControlRecibido controlRecibido)
        {
            if (_context.Controles == null)
            {
                return Problem("El recurso 'Controles' no está disponible.");
            }

            // Verificar si el producto existe
            var producto = await _context.Producto.Where(x => x.Id == controlRecibido.IdProducto).FirstOrDefaultAsync();
            if (producto == null)
            {
                return BadRequest("El producto con el id especificado no existe.");
            }

            // Crear nuevo control
            var control = new Controles
            {
                IdProductos = controlRecibido.IdProducto,
                Linea = controlRecibido.Linea,
                PaisDestino = controlRecibido.PaisDestino,
                Comentario = controlRecibido.Comentario,
                Tipodecontrol = controlRecibido.TipoDeControl,
                FechaHoraPrimerControl = DateTime.Now, // Se asigna la fecha actual
                Estado = controlRecibido.Estado,
                IdUsuarios = controlRecibido.IdUsuario
                
            };

            _context.Controles.Add(control);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetControles), new { id = control.Id }, control);
        }

        [HttpPost("EditarControl")]
        public async Task<ActionResult<Controles>> EditarControl(ControlUpdate cup)
        {
            if (_context.Controles == null)
            {
                return Problem("El recurso 'Controles' no está disponible.");
            }

            var control = _context.Controles.Where( x => x.Id.Equals(cup.id)).FirstOrDefault();
            if (control != null)
            {
                control.Comentario = cup.comentario;
                control.EstadoFinal = cup.estadoFinal;
                control.FechaHoraControlFinal = DateTime.Now;

                _context.Controles.Update(control);
                _context.SaveChanges();
                return Ok();    

            }
            else { return NotFound(); }

           
        }








        // DELETE: api/ApiControles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteControles(int id)
        {
            if (_context.Controles == null)
            {
                return NotFound("El recurso 'Controles' no está disponible.");
            }

            var controles = await _context.Controles.FindAsync(id);
            if (controles == null)
            {
                return NotFound($"Control con id {id} no encontrado.");
            }

            _context.Controles.Remove(controles);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ControlesExists(int id)
        {
            return _context.Controles?.Any(e => e.Id == id) ?? false;
        }
    }
}
