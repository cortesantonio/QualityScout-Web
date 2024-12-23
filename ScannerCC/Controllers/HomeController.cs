using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityScout.Models;
using ScannerCC.Models;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;


namespace ScannerCC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult RedirigirSegunRol()
        {
            if (User.Identity.IsAuthenticated) 
            {
                var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                if (roles.Contains("Especialista"))
                {
                    return RedirectToAction("Index", "Especialista");
                }
                else if (roles.Contains("Control de Calidad"))
                {
                    return RedirectToAction("Index", "ControlCalidad");
                }
            }

            // Si no tiene sesión activa o no tiene un rol válido
            return RedirectToAction("Index2", "Home");
        }

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

        public async Task<IActionResult> Index()
        {
            var stats = await GetInfoStats();

            // Indicadores de rendimiento (rechazados, aprobados, reprocesos)
            var totalControles = _context.Controles.Count();
            var indicadoresRendimiento = _context.Controles
                .GroupBy(c => c.Estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count()
                })
                .ToList();
            ViewBag.IndicadoresRendimiento = indicadoresRendimiento;

            return View(stats);
        }

        public IActionResult Index1()
        {
            return View();
        }

        public IActionResult Index2()
        {
            var rol = _context.Rol.ToList().Count;
            if (rol == 0) {
                string[] roles = { "Control de Calidad", "Especialista"};
                for (int i = 0; i < roles.Length; i++)
                {
                    Rol R = new Rol();
                    R.Nombre = roles[i];
                    _context.Rol.Add(R);
                    _context.SaveChanges();
                }
            }
            var Users = _context.Usuario.ToList().Count;
            if (Users == 0)
            {
                return RedirectToAction("CreateAdminUser", "Auth");
            }


            if (User.Identity.IsAuthenticated)
            {
                var trab = _context.Usuario.Include(r => r.Rol).Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
                
                if (trab != null && trab.Rol.Nombre == "Control de Calidad")
                {
                    return RedirectToAction("Index", "Controlcalidad");
                }
                if (trab != null && trab.Rol.Nombre == "Especialista")
                {
                    return RedirectToAction("Index", "Especialista");
                }
            }
            else
            {
                return View();
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}