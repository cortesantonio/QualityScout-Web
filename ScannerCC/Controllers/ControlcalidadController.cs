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

            // Obtener la fecha del mes más antiguo
            DateTime fechaAntigua;
            if (await _context.Controles.AnyAsync(c => c.FechaHoraPrimerControl != null))
            {
                fechaAntigua = await _context.Controles.MinAsync(c => c.FechaHoraPrimerControl);
            }
            else
            {
                fechaAntigua = DateTime.Now;  // Valor por defecto
            }

            // Cálculo de controles en el mes más antiguo
            var controlesAntiguoAprobados = await _context.Controles
                .CountAsync(c => c.Estado == "Aprobado" &&
                                 c.FechaHoraPrimerControl.Month == fechaAntigua.Month &&
                                 c.FechaHoraPrimerControl.Year == fechaAntigua.Year);

            var controlesAntiguoReprocesados = await _context.Controles
                .CountAsync(c => c.Estado == "Reproceso" &&
                                 c.FechaHoraPrimerControl.Month == fechaAntigua.Month &&
                                 c.FechaHoraPrimerControl.Year == fechaAntigua.Year);

            var controlesAntiguoRechazados = await _context.Controles
                .CountAsync(c => c.Estado == "Rechazado" &&
                                 c.FechaHoraPrimerControl.Month == fechaAntigua.Month &&
                                 c.FechaHoraPrimerControl.Year == fechaAntigua.Year);

            // Calcular el total de controles en el mes antiguo
            var totalControlesMesAntiguo = controlesAntiguoAprobados + controlesAntiguoReprocesados + controlesAntiguoRechazados;

            // Función para calcular porcentajes
            string CalcularPorcentaje(int controlesAntiguo, int totalControlesMesAntiguo)
            {
                if (controlesAntiguo == 0 || totalControlesMesAntiguo == 0)
                    return "0%";

                var porcentaje = (decimal)controlesAntiguo / totalControlesMesAntiguo * 100;
                return $"{Math.Round(porcentaje, 0)}%";
            }

            return new InfoViewModel
            {
                ControlesAprobados = controlesAprobados,
                ControlesReprocesados = controlesReprocesados,
                ControlesRechazados = controlesRechazados,
                AprobadosMesAntiguo = CalcularPorcentaje(controlesAntiguoAprobados, totalControlesMesAntiguo),
                ReprocesadosMesAntiguo = CalcularPorcentaje(controlesAntiguoReprocesados, totalControlesMesAntiguo),
                RechazadosMesAntiguo = CalcularPorcentaje(controlesAntiguoRechazados, totalControlesMesAntiguo),
                MesAnterior = fechaAntigua.ToString("MMMM", new CultureInfo("es-ES")),
                AnioAnterior = fechaAntigua.Year
            };
        }


        // GET: ControlcalidadController
        [Authorize(Roles = "Control de Calidad")]
        public async Task<IActionResult> Index(string Busqueda)
        {
            var stats = new InfoViewModel();
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo; 

            DateTime fechaHoy = DateTime.Now;

            if (User.Identity.IsAuthenticated)
            {

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
                stats = await GetInfoStats();
            }
            return View(stats);
        }
    }
}
