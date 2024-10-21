using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // GET: Productos/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Producto == null)
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

            return View(producto);
        }

        // GET: Productos/Create
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

        // POST: Productos/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string CodigoBarra, string CodigoVE, string Nombre, string URLImagen, string PaisDestino, int IdInformacionQuimica, string Idioma, string UnidadMedida, string DescripcionCapsula)
        {
            try
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
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el producto: " + ex.Message);
            }
        }

        // GET: Productos/Edit
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
            ViewData["IdInformacionQuimica"] = new SelectList(infquimica, "Id", "DisplayInfo", producto?.IdInformacionQuimica);
            return View(producto); 
        }

        // POST: Productos/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CodigoBarra, string CodigoVE, string Nombre, string URLImagen, string PaisDestino, int IdInformacionQuimica, string Idioma, string UnidadMedida, string DescripcionCapsula)
        {
            try
            {
                var producto = await _context.Producto.FindAsync(id);
                if (producto == null)
                {
                    return NotFound("Producto no encontrado.");
                }

                producto.CodigoBarra = CodigoBarra;
                producto.CodigoVE = CodigoVE;
                producto.Nombre = Nombre;
                producto.URLImagen = URLImagen;
                producto.PaisDestino = PaisDestino;
                producto.Idioma = Idioma;
                producto.IdInformacionQuimica = IdInformacionQuimica;
                producto.UnidadMedida = UnidadMedida;
                producto.DescripcionCapsula = DescripcionCapsula;

                _context.Update(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar el producto: " + ex.Message);
            }
        }

        // GET: Productos/Desactivar
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

        // POST: Productos/Desactivar
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            try
            {
                var producto = await _context.Producto.FindAsync(id);
                if (producto == null)
                {
                    return NotFound("Producto no encontrado.");
                }

                producto.Activo = false;

                _context.Update(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al desactivar el producto: " + ex.Message);
            }
        }

        // GET: Productos/Activar
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

        // POST: Productos/Activar
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            try
            {
                var producto = await _context.Producto.FindAsync(id);
                if (producto == null)
                {
                    return NotFound("Producto no encontrado.");
                }

                producto.Activo = true;

                _context.Update(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al activar el producto: " + ex.Message);
            }
        }

        private bool ProductoExists(int id)
        {
            return (_context.Producto?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
