using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class ControlesController : Controller
    {
        private readonly AppDbContext _context;

        public ControlesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Controles
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Controles.Include(c => c.Productos);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Controles/Details/5
        public async Task<IActionResult> Details2(int? id)
        {
            if (id == null || _context.Controles == null)
            {
                return NotFound();
            }

            var controles = await _context.Controles
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (controles == null)
            {
                return NotFound();
            }

            return View(controles);
        }

        // GET: Controles/Create
        public IActionResult CreateControl() {

            var productos = _context.Producto.Select(p => new { p.Id }).ToList();
            var usuarios = _context.Usuario.Select(u => new { u.Id }).ToList();

            ViewData["IdProductos"] = new SelectList(productos, "Id", "Id");
            ViewData["IdUsuarios"] = new SelectList(usuarios, "Id", "Id");
            return View();
        }

        // POST: Controles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateControl([Bind("Id,IdProductos,Linea,PaisDestino,Comentario,Tipodecontrol,FechaHoraPrimerControl,Estado,EstadoFinal,IdUsuarios,FechaHoraControlFinal")] Controles controles)
        {
           
                _context.Add(controles);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Home");
            
            
        }

        // GET: Controles/Edit/5
        public async Task<IActionResult> Edit2(int? id)
        {
            if (id == null || _context.Controles == null)
            {
                return NotFound();
            }

            var controles = await _context.Controles.FindAsync(id);
            if (controles == null)
            {
                return NotFound();
            }

            ViewData["IdProductos"] = new SelectList(_context.Producto, "Id", "Id", controles.IdProductos);
            ViewData["IdUsuarios"] = new SelectList(_context.Usuario, "Id", "Id", controles.IdUsuarios);
            return View(controles);
        }

        // POST: Controles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(int id, [Bind("Id,IdProductos,Linea,PaisDestino,Comentario,Tipodecontrol,FechaHoraPrimerControl,Estado,EstadoFinal,IdUsuarios,FechaHoraControlFinal")] Controles controles)
        {
            if (id != controles.Id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(controles);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ControlesExists(controles.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Home");
            
            
        }

        // GET: Controles/Delete/5
        public async Task<IActionResult> Delete2(int? id)
        {
            if (id == null || _context.Controles == null)
            {
                return NotFound();
            }

            var controles = await _context.Controles
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (controles == null)
            {
                return NotFound();
            }

            return View(controles);
        }

		


		// POST: Controles/Delete/5
		[HttpPost]
        public IActionResult DeleteConfirmed(int id)
        { 
            var control =  _context.Controles.Where(x=>x.Id == id).FirstOrDefault();
            _context.Controles.Remove(control);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
            
        }

        private bool ControlesExists(int id)
        {
          return (_context.Controles?.Any(e => e.Id == id)).GetValueOrDefault();
        }


		[HttpGet]
		[Route("Productos/getData")]
		public IActionResult GetInfo()
		{
			var data = _context.Producto.ToList();

			return Json(data);
		}
	}


}
