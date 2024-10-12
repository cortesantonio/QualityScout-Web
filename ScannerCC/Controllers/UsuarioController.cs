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
            return View();
        }

        // POST: Usuarios/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Rut,Email,RolId")] Usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Activo = true; 
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(usuario);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Rut,Email,RolId")] Usuarios usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
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
        [HttpPost, ActionName("Desactivar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarConfirmado(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false; 
                _context.Update(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), "Home");
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
        [HttpPost, ActionName("Activar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarConfirmado(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = true; 
                _context.Update(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuario?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
