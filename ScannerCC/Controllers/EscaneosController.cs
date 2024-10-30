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

        public IActionResult GestionEscaneos()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            ViewBag.Escaneos = _context.Escaneo.ToList();
            ViewBag.Usuarios = _context.Usuario.Include(r => r.Rol).ToList();
            ViewBag.Productos = _context.Producto.ToList();
            return View();
        }
    }
}
