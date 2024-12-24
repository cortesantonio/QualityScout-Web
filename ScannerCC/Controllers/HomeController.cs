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

        public async Task<IActionResult> Index()
        {
            var stats = await GetInfoStats();

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