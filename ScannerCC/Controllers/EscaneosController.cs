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

        [Authorize(Roles = "Especialista")]
        public IActionResult GestionEscaneos(string orderByDate, string orderByProductName)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;
            // Obtén la lista de escaneos y aplica las condiciones de ordenamiento
            var escaneos = _context.Escaneo
                .Include(e => e.Productos) // Incluye la relación con Productos
                .AsQueryable();

            // Ordenar por nombre de producto
            if (!string.IsNullOrEmpty(orderByProductName))
            {
                escaneos = orderByProductName == "asc"
                    ? escaneos.OrderBy(e => e.Productos.Nombre)
                    : escaneos.OrderByDescending(e => e.Productos.Nombre);
            }
            // Ordenar por fecha
            else if (!string.IsNullOrEmpty(orderByDate))
            {
                escaneos = orderByDate == "asc"
                    ? escaneos.OrderBy(e => e.Fecha)
                    : escaneos.OrderByDescending(e => e.Fecha);
            }

            ViewBag.Escaneos = escaneos.ToList();
            ViewBag.Usuarios = _context.Usuario.Include(r => r.Rol).ToList();
            ViewBag.Productos = _context.Producto.ToList();
            return View();
        }
    }
}
