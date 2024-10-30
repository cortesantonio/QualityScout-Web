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

        private async Task<InfoViewModel> GetInfoStats()
        {
            // Cálculo de totales actuales para controles
            var controlesAprobados = _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Aprobado"))
                .Count();

            var controlesReprocesados = _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Reproceso"))
                .Count();

            var controlesRechazados = _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Rechazado"))
                .Count();

            // Obtener la fecha del mes más antiguo
            DateTime fechaAntigua;

            if (await _context.Controles.AnyAsync(c => c.FechaHoraPrimerControl != null))
            {
                fechaAntigua = await _context.Controles.MinAsync(c => c.FechaHoraPrimerControl);
            }
            else
            {
                fechaAntigua = DateTime.Now;  // O algún valor por defecto que prefieras
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

            // Función para calcular porcentajes
            string CalcularPorcentaje(int controlesAntiguo, int totalControles)
            {
                if (controlesAntiguo == 0 || totalControles == 0)
                    return "0%";

                var porcentaje = (decimal)controlesAntiguo / totalControles * 100;
                return $"{Math.Round(porcentaje, 0)}%";
            }

            return new InfoViewModel
            {
                ControlesAprobados = controlesAprobados,
                ControlesReprocesados = controlesReprocesados,
                ControlesRechazados = controlesRechazados,
                AprobadosMesAntiguo = CalcularPorcentaje(controlesAntiguoAprobados, controlesAprobados),
                ReprocesadosMesAntiguo = CalcularPorcentaje(controlesAntiguoReprocesados, controlesReprocesados),
                RechazadosMesAntiguo = CalcularPorcentaje(controlesAntiguoRechazados, controlesRechazados),
                MesAnterior = fechaAntigua.ToString("MMMM", new CultureInfo("es-ES")),
                AnioAnterior = fechaAntigua.Year
            };
        }

        // GET: ControlcalidadController
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
