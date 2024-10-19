using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class ControlcalidadController : Controller
    {
        private readonly AppDbContext _context;

        public ControlcalidadController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ControlcalidadController
        public ActionResult Index(string Busqueda)
        {
            DateTime fechaHoy = DateTime.Now;

            if (User.Identity.IsAuthenticated)
            {
                var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
                ViewBag.trab = TrabajadorActivo;

                ViewBag.Usuarios = _context.Usuario.Include(r => r.Rol).ToList();
                ViewBag.Productos = _context.Producto.ToList();
                ViewBag.Controles = _context.Controles.Include(c => c.Productos);

                //Si se realiza busqueda de productos evalua y filtra datos
                if (Busqueda != null)
                {
                    var ProductoResultado = _context.Producto.Where(x => x.Nombre.Contains(Busqueda) || x.CodigoBarra.Contains(Busqueda)).ToList();
                    ViewBag.Productos = ProductoResultado;

                    if (ProductoResultado.Count > 0)
                    {
                        Escaneos E = new Escaneos();
                        E.IdProductos = ProductoResultado.FirstOrDefault().Id;
                        E.IdUsuarios = TrabajadorActivo.Id;
                        E.Fecha = fechaHoy.Date;
                        E.Hora = fechaHoy.TimeOfDay;
                        _context.Escaneo.Add(E);
                        _context.SaveChanges();
                    }
                }
                return View();
            }
            else { return RedirectToAction("Index", "Home"); }
        }
    }
}
