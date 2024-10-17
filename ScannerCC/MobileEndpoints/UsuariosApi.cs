using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityScout.Models;
using ScannerCC.Models;

namespace QualityScout.MobileEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosApi(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UsuariosApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioToSend>>> GetUsuario([FromHeader(Name = "Authorization")] string authorization)
        {
            if (_context.Usuario == null)
            {
                return NotFound();
            }

            // Extrae el token de la cabecera Authorization
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized("Token no proporcionado o no válido.");
            }

            var token = authorization.Substring("Bearer ".Length).Trim();

            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Token == token);
            if (usuario == null)
            {
                return Unauthorized("Token no válido.");
            }

            var usuarios = await _context.Usuario.Include(x => x.Rol).ToListAsync();

            var usuariosToSend = usuarios.Select(usuario => new UsuarioToSend
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Rut = usuario.Rut,
                Email = usuario.Email,
                NombreRol = usuario.Rol.Nombre,
                Token = usuario.Token ,
                Activo = usuario.Activo

            }).ToList();

            return Ok(usuariosToSend);
        }




        // GET: api/UsuariosApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
          if (_context.Usuario == null)
          {
              return NotFound();
          }
            var usuario = await _context.Usuario.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }




        [HttpPut("UpdateActivo/{rut}")]
        public async Task<IActionResult> UpdateActivo(string rut)
        {
            if (_context.Usuario == null)
            {
                return Problem("Entity set 'AppDbContext.Usuario' is null.");
            }

            var usuario = await _context.Usuario.Where(x => x.Rut == rut).FirstOrDefaultAsync();

            if(usuario.Activo == false)
            {
                var nuevoEstado = true;
                usuario.Activo = nuevoEstado;

            }
            else
            {
                var nuevoEstado = false;
                usuario.Activo = nuevoEstado;

            }


            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Actualiza el estado de Activo

            try
            {
                _context.Usuario.Update(usuario);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Estado de usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el estado del usuario: {ex.Message}");
            }
        }







        // DELETE: api/UsuariosApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (_context.Usuario == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuario?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
