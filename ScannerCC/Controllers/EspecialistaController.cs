using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityScout.Models;
using ScannerCC.Models;
using System.Globalization;
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

        [Authorize(Roles = "Especialista")]
        public IActionResult Dashboard()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            // Gráfico de barras - Productos por país de destino
            var productosPorPais = _context.Producto
                .GroupBy(p => p.PaisDestino)
                .Select(g => new { Pais = g.Key, Cantidad = g.Count() })
                .ToList();
            ViewBag.ProductosPorPais = productosPorPais;

            // Gráfico de líneas - Rechazos mensuales
            var rechazosMensuales = _context.Controles
                .Where(c => c.Estado == "Rechazado")
                .GroupBy(c => c.FechaHoraPrimerControl.Month)
                .Select(g => new { Mes = g.Key, Rechazos = g.Count() })
                .OrderBy(x => x.Mes)
                .ToList();
            var mesesNombres = rechazosMensuales.Select(r => new
            {
                Mes = new DateTime(2024, r.Mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                Rechazos = r.Rechazos
            }).ToList();
            ViewBag.RechazosMensuales = mesesNombres;

            // Gráfico de pastel - Variedad de productos
            var variedades = _context.Producto
                .GroupBy(p => p.Nombre)
                .Select(g => new
                {
                    Nombre = g.Key,
                    Cantidad = g.Count()
                })
                .ToList();
            int totalProductos = variedades.Sum(v => v.Cantidad);
            var variedadesConPorcentaje = variedades.Select(v => new
            {
                v.Nombre,
                Porcentaje = (double)v.Cantidad / totalProductos * 100
            }).ToList();
            ViewBag.Variedades = variedadesConPorcentaje;

            // Gráfico de barras - Rechazos por país
            var rechazosPorPais = _context.Controles
                .Where(c => c.Estado == "Rechazado")
                .GroupBy(c => c.PaisDestino)
                .Select(g => new { Pais = g.Key, Cantidad = g.Count() })
                .ToList();

            ViewBag.RechazosPorPais = rechazosPorPais;

            // Gráfico de líneas - Producción mensual
            var produccionMensual = _context.Controles
                .Where(c => c.FechaHoraPrimerControl != null)
                .GroupBy(c => c.FechaHoraPrimerControl.Month)
                .Select(g => new { Mes = g.Key, Produccion = g.Count() })
                .OrderBy(x => x.Mes)
                .ToList();
            var mesesNombresProduccion = produccionMensual.Select(p => new
            {
                Mes = new DateTime(2024, p.Mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                Produccion = p.Produccion
            }).ToList();
            ViewBag.ProduccionMensual = mesesNombresProduccion;

            // Obtener países de destino
            var paisesDestino = _context.Producto
                .Where(p => p.PaisDestino != null)
                .GroupBy(p => p.PaisDestino)
                .Select(g => new
                {
                    Pais = g.Key,
                    Cantidad = g.Count()
                })
                .ToList();
            ViewBag.PaisesDestino = paisesDestino;

            return View();
        }

        [Authorize(Roles = "Especialista")]
        private async Task<InfoViewModel> GetInfoStats()
        {
            // Obtener la fecha actual y el mes anterior
            DateTime fechaMesActual = DateTime.Now;
            DateTime fechaMesAnterior = fechaMesActual.AddMonths(-1);

            // Calcular los controles del mes actual
            var controlesAprobadosMesActual = await _context.Controles
                .Where(c => c.Estado == "Aprobado" &&
                            c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                            c.FechaHoraPrimerControl.Year == fechaMesActual.Year)
                .CountAsync();

            var controlesReprocesadosMesActual = await _context.Controles
                .Where(c => c.Estado == "Reproceso" &&
                            c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                            c.FechaHoraPrimerControl.Year == fechaMesActual.Year)
                .CountAsync();

            var controlesRechazadosMesActual = await _context.Controles
                .Where(c => c.Estado == "Rechazado" &&
                            c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                            c.FechaHoraPrimerControl.Year == fechaMesActual.Year)
                .CountAsync();

            // Calcular los controles del mes anterior
            var controlesMesAnteriorAprobados = await _context.Controles
                .Where(c => c.Estado == "Aprobado" &&
                            c.FechaHoraPrimerControl.Month == fechaMesAnterior.Month &&
                            c.FechaHoraPrimerControl.Year == fechaMesAnterior.Year)
                .CountAsync();

            var controlesMesAnteriorReprocesados = await _context.Controles
                .Where(c => c.Estado == "Reproceso" &&
                            c.FechaHoraPrimerControl.Month == fechaMesAnterior.Month &&
                            c.FechaHoraPrimerControl.Year == fechaMesAnterior.Year)
                .CountAsync();

            var controlesMesAnteriorRechazados = await _context.Controles
                .Where(c => c.Estado == "Rechazado" &&
                            c.FechaHoraPrimerControl.Month == fechaMesAnterior.Month &&
                            c.FechaHoraPrimerControl.Year == fechaMesAnterior.Year)
                .CountAsync();

            // Función para calcular la variación 
            string CalcularVariacion(int actual, int anterior)
            {
                double variacion;
                if (anterior == 0)
                {
                    variacion = actual > 0 ? 100 : 0;
                }
                else
                {
                    variacion = ((double)(actual - anterior) / anterior) * 100;
                }
                return $"{Math.Round(variacion, 2)}%";
            }

            // Calcular las variaciones 
            var variacionAprobados = CalcularVariacion(controlesAprobadosMesActual, controlesMesAnteriorAprobados);
            var variacionReprocesados = CalcularVariacion(controlesReprocesadosMesActual, controlesMesAnteriorReprocesados);
            var variacionRechazados = CalcularVariacion(controlesRechazadosMesActual, controlesMesAnteriorRechazados);

            return new InfoViewModel
            {
                AprobadosMesActual = $"{controlesAprobadosMesActual}",
                ReprocesadosMesActual = $"{controlesReprocesadosMesActual}",
                RechazadosMesActual = $"{controlesRechazadosMesActual}",

                AprobadosMesAntiguo = $"{controlesMesAnteriorAprobados}",
                ReprocesadosMesAntiguo = $"{controlesMesAnteriorReprocesados}",
                RechazadosMesAntiguo = $"{controlesMesAnteriorRechazados}",

                VariacionAprobados = $"{variacionAprobados}",
                VariacionReprocesados = $"{variacionReprocesados}",
                VariacionRechazados = $"{variacionRechazados}",

                MesAnterior = fechaMesAnterior.ToString("MMMM", new CultureInfo("es-ES")).ToUpper(),
                AnioAnterior = fechaMesAnterior.Year
            };
        }

        // GET: AdministradorController
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Index(string Busqueda, string BusquedaUsuarios)
        {
            var stats = new InfoViewModel();

            if (User.Identity.IsAuthenticated)
            {
                DateTime fechaHoy = DateTime.Now;

                // OBTENER INFORMACIOON DE USUARIO ACTIVO EN BASE DE DATOS.
                var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
                ViewBag.trab = TrabajadorActivo;

                ViewBag.Usuarios= _context.Usuario.Include(r => r.Rol).ToList();
                ViewBag.Productos = _context.Producto.ToList();
                ViewBag.BotellaDetalles = _context.BotellaDetalle.ToList();
                ViewBag.InformacionQuimica = _context.InformacionQuimica.ToList();
                ViewBag.Controles = _context.Controles.ToList();
                ViewBag.Informes = _context.Informe.ToList();
                ViewBag.ProductoHistorial = _context.ProductoHistorial.ToList();
                ViewBag.ProductoDetalles = _context.ProductoDetalle.ToList();
                ViewBag.Escaneos = _context.Escaneo.ToList();

                // Obtener la fecha actual
                DateTime fechaMesActual = DateTime.Now;

                var totalControles = _context.Controles
                    .Where(c => c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                                c.FechaHoraPrimerControl.Year == fechaMesActual.Year)
                    .Count();

                var indicadoresRendimiento = _context.Controles
                    .Where(c => c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                                c.FechaHoraPrimerControl.Year == fechaMesActual.Year)
                    .GroupBy(c => c.Estado)
                    .Select(g => new
                    {
                        Estado = g.Key,
                        Porcentaje = (double)g.Count() / totalControles * 100
                    })
                    .ToList();
                ViewBag.IndicadoresRendimiento = indicadoresRendimiento;

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

                stats = await GetInfoStats();
            }
            return View(stats); 
        }

       
    }
}
