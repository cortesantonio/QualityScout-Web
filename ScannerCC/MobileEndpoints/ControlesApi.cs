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
    public class ControlesApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public ControlesApi(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ControlesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Controles>>> GetControles()
        {
          if (_context.Controles == null)
          {
              return NotFound();
          }
            return await _context.Controles.ToListAsync();
        }

        // GET: api/ControlesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Controles>> GetControles(int id)
        {
          if (_context.Controles == null)
          {
              return NotFound();
          }
            var controles = await _context.Controles.FindAsync(id);

            if (controles == null)
            {
                return NotFound();
            }

            return controles;
        }

        // PUT: api/ControlesApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutControles(int id, Controles controles)
        {
            if (id != controles.Id)
            {
                return BadRequest();
            }

            _context.Entry(controles).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ControlesExists(id))
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

        // POST: api/ControlesApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Controles>> PostControles(Controles controles)
        {
          if (_context.Controles == null)
          {
              return Problem("Entity set 'AppDbContext.Controles'  is null.");
          }
            _context.Controles.Add(controles);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetControles", new { id = controles.Id }, controles);
        }

        // DELETE: api/ControlesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteControles(int id)
        {
            if (_context.Controles == null)
            {
                return NotFound();
            }
            var controles = await _context.Controles.FindAsync(id);
            if (controles == null)
            {
                return NotFound();
            }

            _context.Controles.Remove(controles);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ControlesExists(int id)
        {
            return (_context.Controles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
