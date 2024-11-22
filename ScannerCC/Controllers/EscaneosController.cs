using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class EscaneosController : Controller
    {
        private readonly AppDbContext _context;

        public EscaneosController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Especialista, Control de Calidad")]
        public IActionResult GestionEscaneos(string orderByDate, string orderByProductName)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            ViewBag.Escaneos = _context.Escaneo.Include(x => x.Productos).Include(x => x.Usuarios).ToList();

            // Ordenar por nombre de producto
            if (!string.IsNullOrEmpty(orderByProductName))
            {
                if(orderByProductName == "asc")
                {
                    ViewBag.Escaneos = _context.Escaneo.Include(x => x.Productos).Include(x => x.Usuarios).OrderBy(e => e.Productos.Nombre).ToList();
                }
                else
                {
                    ViewBag.Escaneos = _context.Escaneo.Include(x => x.Productos).Include(x => x.Usuarios).OrderByDescending(e => e.Productos.Nombre).ToList();
                }
            }
            // Ordenar por fecha
            else if (!string.IsNullOrEmpty(orderByDate))
            {
                if (orderByDate == "asc")
                {
                    ViewBag.Escaneos= _context.Escaneo.Include(x => x.Productos).Include(x => x.Usuarios).OrderBy(e => e.Productos.FechaRegistro).ToList();
                }
                else
                {
                    ViewBag.Escaneos= _context.Escaneo.Include(x => x.Productos).Include(x => x.Usuarios).OrderByDescending(e => e.Productos.FechaRegistro).ToList();
                }
            }

            ViewBag.Usuarios = _context.Usuario.Include(r => r.Rol).ToList();
            ViewBag.Productos = _context.Producto.ToList();
            return View();
        }

        public IActionResult Scanner()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;
            return View();
        }

    }

}
