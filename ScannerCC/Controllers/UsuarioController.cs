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
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios/Details/5
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            // Ya no se filtra por Activo
            var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            var rols = _context.Rol.Select(bd => new { bd.idRol, bd.Nombre }).ToList();
            ViewData["Roles"] = new SelectList(rols,  "idRol", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Nombre, string Rut, string Email, int RolId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Usuarios usuario = new Usuarios();
                    usuario.Nombre = Nombre;
                    usuario.Rut = Rut;
                    usuario.Email = Email;
                    usuario.RolId = RolId;
                    usuario.Activo = true; // Asigna el estado activo por defecto

                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el usuario: " + ex.Message);
            }
        }


        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Nombre, string Rut, string Email, int RolId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Obtener el usuario a editar
                    var usuario = await _context.Usuario.FindAsync(id);
                    if (usuario == null)
                    {
                        return NotFound("Usuario no encontrado.");
                    }

                    // Asignar propiedades del usuario
                    usuario.Nombre = Nombre;
                    usuario.Rut = Rut;
                    usuario.Email = Email;
                    usuario.RolId = RolId;

                    // Guardar cambios
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar el usuario: " + ex.Message);
            }
        }

        // GET: Usuarios/Desactivar/5
        public async Task<IActionResult> Desactivar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario); 
        }

        // POST: Usuarios/Desactivar/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            try
            {
                // Obtener el usuario a desactivar
                var usuario = await _context.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                // Marcar el usuario como inactivo
                usuario.Activo = false;

                // Guardar cambios
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al desactivar el usuario: " + ex.Message);
            }
        }

        // GET: Usuarios/Activar/5
        public async Task<IActionResult> Activar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario); 
        }

        // POST: Usuarios/Activar/5
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            try
            {
                // Obtener el usuario a activar
                var usuario = await _context.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                // Marcar el usuario como activo
                usuario.Activo = true;

                // Guardar cambios
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al activar el usuario: " + ex.Message);
            }
        }


        private bool UsuarioExists(int id)
        {
            return (_context.Usuario?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
