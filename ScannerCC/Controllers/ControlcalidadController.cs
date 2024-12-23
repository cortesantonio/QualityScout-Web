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
            // Cálculo de totales actuales para controles
            var controlesAprobados = await _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Aprobado"))
                .CountAsync();

            var controlesReprocesados = await _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Reproceso"))
                .CountAsync();

            var controlesRechazados = await _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Rechazado"))
                .CountAsync();

            // Obtener la fecha del mes actual
            DateTime fechaMesActual = DateTime.Now;

            // Cálculo de controles en el mes actual
            var controlesMesActualAprobados = await _context.Controles
                .CountAsync(c => c.Estado == "Aprobado" &&
                                 c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                                 c.FechaHoraPrimerControl.Year == fechaMesActual.Year);

            var controlesMesActualReprocesados = await _context.Controles
                .CountAsync(c => c.Estado == "Reproceso" &&
                                 c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                                 c.FechaHoraPrimerControl.Year == fechaMesActual.Year);

            var controlesMesActualRechazados = await _context.Controles
                .CountAsync(c => c.Estado == "Rechazado" &&
                                 c.FechaHoraPrimerControl.Month == fechaMesActual.Month &&
                                 c.FechaHoraPrimerControl.Year == fechaMesActual.Year);

            // Calcular el total de controles en el mes actual
            var totalControlesMesActual = controlesMesActualAprobados + controlesMesActualReprocesados + controlesMesActualRechazados;

            // Función para calcular porcentajes
            string CalcularPorcentaje(int controlesMesActual, int totalControlesMesActual)
            {
                if (controlesMesActual == 0 || totalControlesMesActual == 0)
                    return "0%";

                var porcentaje = (decimal)controlesMesActual / totalControlesMesActual * 100;
                return $"{Math.Round(porcentaje, 0)}%";
            }

            return new InfoViewModel
            {
                ControlesAprobados = controlesAprobados,
                ControlesReprocesados = controlesReprocesados,
                ControlesRechazados = controlesRechazados,
                AprobadosMesAntiguo = CalcularPorcentaje(controlesMesActualAprobados, totalControlesMesActual),
                ReprocesadosMesAntiguo = CalcularPorcentaje(controlesMesActualReprocesados, totalControlesMesActual),
                RechazadosMesAntiguo = CalcularPorcentaje(controlesMesActualRechazados, totalControlesMesActual),
                MesAnterior = fechaMesActual.ToString("MMMM", new CultureInfo("es-ES")).ToUpper(),
                AnioAnterior = fechaMesActual.Year
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

                // Indicadores de rendimiento (rechazados, aprobados, reprocesos)
                var totalControles = _context.Controles.Count();
                var indicadoresRendimiento = _context.Controles
                    .GroupBy(c => c.Estado)
                    .Select(g => new
                    {
                        Estado = g.Key,
                        Cantidad = g.Count()
                    })
                    .ToList(); // agrupa los controles por estado y calcula el porcentaje de cada uno sobre el total.
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
