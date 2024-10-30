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

        public IActionResult GestionUsuarios(string Busqueda)
        {
            // Filtrar usuarios por Rut o Nombre
            var usuarios = _context.Usuario.AsQueryable();
            if (!string.IsNullOrEmpty(Busqueda))
            {
                usuarios = usuarios.Where(u => u.Rut.Contains(Busqueda) || u.Nombre.Contains(Busqueda));
            }
            ViewBag.Usuarios = usuarios.ToList(); // Guardamos la lista de usuarios filtrados

            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            ViewBag.Usuario = _context.Usuario.Include(r => r.Rol).ToList();
            return View();
        }

        // GET: Usuarios/Details
        public async Task<IActionResult> Details(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            var usuarios = _context.Usuario.FirstOrDefault(u => u.Id == id); 
            ViewBag.UsuarioId = usuarios.Id;

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
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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

        // GET: Usuarios/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }
            var usuarios = _context.Usuario.FirstOrDefault(u => u.Id == id);
            ViewBag.UsuarioId = usuarios.Id;

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var rols = _context.Rol.Select(bd => new { bd.idRol, bd.Nombre }).ToList();
            ViewData["Roles"] = new SelectList(rols, "idRol", "Nombre");

            return View(usuario);
        }

        // POST: Usuarios/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Nombre, string Rut, string Email, int RolId)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            var usuarios = _context.Usuario.FirstOrDefault(u => u.Id == id);
            ViewBag.UsuarioId = usuarios.Id;

            try
            {
                if (ModelState.IsValid)
                {
                    var usuario = await _context.Usuario.FindAsync(id);
                    if (usuario == null)
                    {
                        return NotFound("Usuario no encontrado.");
                    }

                    usuario.Nombre = Nombre;
                    usuario.Rut = Rut;
                    usuario.Email = Email;
                    usuario.RolId = RolId;

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

        // GET: Usuarios/Edit
        public async Task<IActionResult> EditC(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var rols = _context.Rol.Select(bd => new { bd.idRol, bd.Nombre }).ToList();
            ViewData["Roles"] = new SelectList(rols, "idRol", "Nombre");

            return View(usuario);
        }

        // POST: Usuarios/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditC(int id, int Rol)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                if (ModelState.IsValid)
                {
                    var usuario = await _context.Usuario.FindAsync(id);
                    if (usuario == null)
                    {
                        return NotFound("Usuario no encontrado.");
                    }

                    // Aquí puedes utilizar el valor de Rol si es necesario
                    // Por ejemplo, puedes asignar el rol al usuario:
                    usuario.RolId = Rol;

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar la contraseña del usuario: " + ex.Message);
            }
        }

        // GET: Usuarios/Desactivar
        public async Task<IActionResult> Desactivar(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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

        // POST: Usuarios/Desactivar
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var usuario = await _context.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                usuario.Activo = false;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al desactivar el usuario: " + ex.Message);
            }
        }

        // GET: Usuarios/Activar
        public async Task<IActionResult> Activar(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

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

        // POST: Usuarios/Activar
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var usuario = await _context.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                usuario.Activo = true;

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
