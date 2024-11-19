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
    public class InformesApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public InformesApi(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/InformesApi
        [HttpGet]
        public async Task<IActionResult> GetInformes()
        {
            var informes = await _context.Informe
                .OrderByDescending(c => c.Fecha)

                .Select(informe => new
                {
                    id = informe.Id,
                    idUsuarios = informe.IdUsuarios,
                    titulo = informe.Titulo,
                    enfoque = informe.Enfoque,
                    fecha = informe.Fecha,
                    descripcion = informe.Descripcion,
                    usuario = new
                    {
                        id = informe.Usuarios.Id,
                        nombre = informe.Usuarios.Nombre,
                        email = informe.Usuarios.Email // Solo datos no sensibles
                    }
                })
                .ToListAsync();

            return Ok(informes);
        }


        [HttpGet("GetInfoToInforme")]
        public async Task<IActionResult> GetInfoToInforme()
        {
            var controles = _context.Controles
                .Include(x => x.Productos)
                  .OrderByDescending(c => c.FechaHoraPrimerControl) 
                .Take(30) 
                .Select(c => new
                {
                    c.Id,
                    c.Linea,
                    c.PaisDestino,
                    c.Comentario,
                    c.Tipodecontrol,
                    c.FechaHoraPrimerControl,
                    c.Estado,
                    c.FechaHoraControlFinal,
                    c.EstadoFinal,
                    Producto = new
                    {
                        c.Productos.CodigoBarra,
                        c.Productos.Nombre,
                        c.Productos.PaisDestino,
                        c.Productos.Idioma,
                        c.Productos.DescripcionCapsula,

                    }
                })
                .ToList();

            return Ok(controles);
        }




        // GET: api/InformesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Informes>> GetInformes(int id)
        {
          if (_context.Informe == null)
          {
              return NotFound();
          }
            var informes = await _context.Informe.FindAsync(id);

            if (informes == null)
            {
                return NotFound();
            }

            return informes;
        }

        // PUT: api/InformesApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInformes(int id, Informes informes)
        {
            if (id != informes.Id)
            {
                return BadRequest();
            }

            _context.Entry(informes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InformesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/InformesApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        public class InformeReciveDTO
        {
            public string Titulo { get; set; }
            public string Enfoque { get; set; }
            public string Descripcion { get; set; }
            public int UsuarioId { get; set; }
        
        }
        
        
        [HttpPost]
        public async Task<ActionResult<Informes>> PostInformes(InformeReciveDTO InformeReciveDTO)
        {
          if (_context.Informe == null)
          {
              return Problem("Entity set 'AppDbContext.Informe'  is null.");
          }

            Informes informe = new Informes();
            informe.Titulo = InformeReciveDTO.Titulo;
            informe.Descripcion  = InformeReciveDTO.Descripcion;
            informe.Enfoque = InformeReciveDTO.Enfoque;
            informe.IdUsuarios = InformeReciveDTO.UsuarioId;
            informe.Fecha = DateTime.Now;

            _context.Informe.Add(informe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInformes", new { id = informe.Id }, informe);
        }

        // DELETE: api/InformesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInformes(int id)
        {
            if (_context.Informe == null)
            {
                return NotFound();
            }
            var informes = await _context.Informe.FindAsync(id);
            if (informes == null)
            {
                return NotFound();
            }

            _context.Informe.Remove(informes);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InformesExists(int id)
        {
            return (_context.Informe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
