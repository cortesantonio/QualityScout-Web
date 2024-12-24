using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityScout.Models;
using ScannerCC.Models;
using System.Globalization;

namespace ScannerCC.Controllers
{
    public class ControlcalidadController : Controller
    {
        private readonly AppDbContext _context;

        public ControlcalidadController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Control de Calidad")]
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

        // GET: ControlcalidadController
        [Authorize(Roles = "Control de Calidad")]
        public async Task<IActionResult> Index(string Busqueda)
        {
            var stats = new InfoViewModel(); // solicita la informacion de estadisticas declaradas arriba.
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo; 

            DateTime fechaHoy = DateTime.Now;

            if (User.Identity.IsAuthenticated)
            {

                ViewBag.Usuarios = _context.Usuario.Include(r => r.Rol).ToList();
                ViewBag.Productos = _context.Producto.ToList();
                ViewBag.Controles = _context.Controles.Include(c => c.Productos);

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
                stats = await GetInfoStats();
            }
            return View(stats);
        }
    }
}
