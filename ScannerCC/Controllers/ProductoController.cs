using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class ProductoController : Controller
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {

            var infquimica = _context.InformacionQuimica
                .Select(iq => new
                {
                    iq.Id,
                    DisplayInfo = String.Format(
                        "Cepa: {0} | Azúcar: {1} - {2} | Sulfuroso: {3} - {4} | Densidad: {5} - {6} | Alcohol: {7} - {8}",
                        iq.Cepa,
                        iq.MinAzucar, iq.MaxAzucar,
                        iq.MinSulfuroso, iq.MaxSulfuroso,
                        iq.MinDensidad, iq.MaxDensidad,
                        iq.MinGradoAlcohol, iq.MaxGradoAlcohol
                    )
                })
                .ToList();

            var usuarios = _context.Usuario.Select(u => new { u.Id, u.Nombre }).ToList(); 

            ViewData["IdInformacionQuimica"] = new SelectList(infquimica, "Id", "DisplayInfo");
            return View();
        }

        // POST: Productoes/Edit/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string CodigoBarra, string CodigoVE, string Nombre, string URLImagen, string PaisDestino, int IdInformacionQuimica, string Idioma, string UnidadMedida, string DescripcionCapsula)
        {
                // Obtener el usuario actualmente logueado
                var currentUser = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.Rut == User.Identity.Name); 
                if (currentUser == null)
                {
                    return Problem("Usuario no encontrado.");
                }

                Productos producto = new Productos();
                producto.CodigoBarra = CodigoBarra;
                producto.CodigoVE = CodigoVE;
                producto.Nombre = Nombre;
                producto.URLImagen = URLImagen;
                producto.PaisDestino = PaisDestino;
                producto.Idioma = Idioma;
                producto.IdInformacionQuimica = IdInformacionQuimica;
                producto.UnidadMedida = UnidadMedida;
                producto.DescripcionCapsula = DescripcionCapsula;
                producto.IdUsuarios = currentUser.Id; // Asigna el Id del usuario logueado
                producto.FechaRegistro = DateTime.Now; // Asigna la fecha actual
                producto.Activo = true; // Por defecto activo

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            

        }


        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var infquimica = _context.InformacionQuimica
                .Select(iq => new
                {
                    iq.Id,
                    DisplayInfo = String.Format(
                        "Cepa: {0} | Azúcar: {1} - {2} | Sulfuroso: {3} - {4} | Densidad: {5} - {6} | Alcohol: {7} - {8}",
                        iq.Cepa,
                        iq.MinAzucar, iq.MaxAzucar,
                        iq.MinSulfuroso, iq.MaxSulfuroso,
                        iq.MinDensidad, iq.MaxDensidad,
                        iq.MinGradoAlcohol, iq.MaxGradoAlcohol
                    )
                })
                .ToList();

            var usuarios = _context.Usuario.Select(u => new { u.Id, u.Nombre }).ToList(); // Incluye nombres para identificación

            ViewData["IdInformacionQuimica"] = new SelectList(infquimica, "Id", "DisplayInfo", producto?.IdInformacionQuimica);
            ViewData["IdUsuarios"] = new SelectList(usuarios, "Id", "Nombre", producto.IdUsuarios); // Mostrar los nombres de usuario

            return View(producto); // Devuelve el producto con los datos actuales
        }


        // POST: Productoes/Edit/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdInformacionQuimica,CodigoBarra,CodigoVE,Nombre,URLImagen,PaisDestino,IdUsuarios,Activo,FechaRegistro,Idioma,UnidadMedida,DescripcionCapsula")] Productos producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            return View(producto);
        }


        // GET: Productoes/Desactivar/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Desactivar(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Desactivar/5
        [Authorize(Roles = "Especialista")]
        [HttpPost, ActionName("Desactivar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarConfirmedo(int id)
        {
            if (_context.Producto == null)
            {
                return Problem("Entity set 'AppDbContext.Producto' is null.");
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                producto.Activo = false; 
                _context.Producto.Update(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Productoes/Activar/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Activar(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Activar/5
        [Authorize(Roles = "Especialista")]
        [HttpPost, ActionName("Activar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarConfirmedo(int id)
        {
            if (_context.Producto == null)
            {
                return Problem("Entity set 'AppDbContext.Producto' is null.");
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                producto.Activo = true; 
                _context.Producto.Update(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool ProductoExists(int id)
        {
            return (_context.Producto?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
