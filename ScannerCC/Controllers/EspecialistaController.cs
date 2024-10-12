using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;
using System.Linq;

namespace ScannerCC.Controllers
{
    public class EspecialistaController : Controller
    {

        private readonly AppDbContext _context;

        public EspecialistaController(AppDbContext context)
        {
            _context = context;
        }
        // GET: AdministradorController
        public ActionResult Index(string Busqueda, string BusquedaUsuarios, int paginaEscaneos = 1, int paginaControles = 1)
        {
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaMesAnterior = fechaHoy.AddMonths(-1);
            DateTime fechaHaceUnAnio = fechaHoy.AddYears(-1);
            DateTime fechaHaceDosAnio = fechaHoy.AddYears(-2);


            if (User.Identity.IsAuthenticated)
            {
                // OBTENER INFORMACIOON DE USUARIO ACTIVO EN BASE DE DATOS.
                var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
                ViewBag.trab = TrabajadorActivo;

                //Consulta de usuarios en base de datos de usuarios con rol'Control de calidad'.
                var Controlcalidad = _context.Usuario
                    .Include(x => x.Rol)
                    .Where(r => r.Rol.Nombre == "Control de calidad").ToList();
                
                ViewBag.countUsuariosControlcalidad = Controlcalidad.Count;
                

                //Consulta de usuarios en base de datos de usuarios con rol'Especialista'.

                var Especialista = _context.Usuario
                    .Include(x => x.Rol)
                    .Where(r => r.Rol.Nombre == "Especialista").ToList();

                ViewBag.countUsuariosEspecialista = Especialista.Count;

                //DATOS PARA GRAFICO DE TORTA DE TIPOS DE ETIQUETAS
                var counts = _context.ProductoDetalle
                    .GroupBy(r => r.TipoEtiqueta)
                    .Select(g => new { Etiqueta = g.Key, Count = g.Count() })
                    .ToList();

                ViewBag.countRose = counts.FirstOrDefault(c => c.Etiqueta == "Rose")?.Count ?? 0;
                ViewBag.countTinto = counts.FirstOrDefault(c => c.Etiqueta == "Tinto")?.Count ?? 0;
                ViewBag.countBlanco = counts.FirstOrDefault(c => c.Etiqueta == "Blanco")?.Count ?? 0;


                // COUNT REGISTROS 
                ViewBag.countProductos = _context.Producto.ToList().Count;
                ViewBag.countUsuarios = _context.Usuario.ToList().Count;
                ViewBag.countEscaneos = _context.Escaneo.ToList().Count;


                ViewBag.produccionDosAnio = _context.ProductoHistorial
                .Where(e => e.FechaProduccion >= fechaHaceDosAnio && e.FechaProduccion <= fechaHaceUnAnio)
                .ToList().Count;
                if (ViewBag.produccionDosAnio == null)
                {
                    ViewBag.produccionDosAnio = 0;
                }
        
                ViewBag.Usuarios= _context.Usuario.Include(r => r.Rol).ToList();
                ViewBag.Productos = _context.Producto.ToList();


                //Datos para la tablaEscaneos

                const int registrosPorPagina = 2;

                var escaneos = _context.Escaneo
                    .Include(e => e.Productos)
                    .Include(e => e.Usuarios)
                    .OrderByDescending(e => e.Fecha)
                    .Skip((paginaEscaneos - 1) * registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToList();

                ViewBag.Escaneos = escaneos;
                ViewBag.PaginaActualEscaneos = paginaEscaneos;

                //Datos adm. controles

                var controles = _context.Controles
                .Include(c => c.Productos)
                .ToList();

                ViewBag.Controles = controles;
                ViewBag.BotellaDetalles = _context.BotellaDetalle.ToList();
                ViewBag.InformacionQuimica = _context.InformacionQuimica.ToList();
                ViewBag.Informes = _context.Informe.ToList();
                ViewBag.ProductoHistorial = _context.ProductoHistorial.ToList();
                ViewBag.ProductoDetalles = _context.ProductoDetalle.ToList();

                //Datos para la tablaControl y sus count

                int totalControles = _context.Controles.Count();

                var controless = _context.Controles
                    .Include(c => c.Productos)
                    .Skip((paginaControles - 1) * registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToList();

                ViewBag.Controless = controless;
                ViewBag.PaginaActualControles = paginaControles;
                ViewBag.TotalControles = totalControles;


                //Si se realiza busqueda de productos evalua y filtra datos
                if (Busqueda != null)
                {
                    var ProductoResultado = _context.Producto.Where(x => x.Nombre.Contains(Busqueda) || x.CodigoBarra.Contains(Busqueda)).ToList();
                    ViewBag.Productos= ProductoResultado;
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

                //buscar usuarios
                if (BusquedaUsuarios != null)
                {
                    ViewBag.Usuarios = _context.Usuario.Where(x => x.Rut == BusquedaUsuarios || x.Nombre == BusquedaUsuarios).ToList();
                }
                return View();
            }
            else { return RedirectToAction("Index", "Home"); }
        }

       
    }
}
