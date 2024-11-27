using Microsoft.AspNetCore.Mvc;
using ScannerCC.Models;


namespace QualityScout.MobileEndpoints
{

    [Route("api/[controller]")]
    [ApiController]
    public class ApiDashboard : ControllerBase
    {
        private readonly AppDbContext _context;
        public ApiDashboard(AppDbContext context)
        {
            _context = context;
        }

        // DESDE AQUI SACAMOS TODOS LOS DATOS PARA LOS INDICACDORES, GRAFICOS, ETC.

        // GET: api/<ApiDashboard>

        [HttpPost("CantidadPorEstado")]
        public async Task<ActionResult<Controles>> CantidadPorEstado()
        {
            var resultado =  _context.Controles
                .GroupBy(x => x.Estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count()
                })
                .ToList();

            return Ok(resultado);
        }
        [HttpPost("RechazadosPais")]
        public async Task<ActionResult<Controles>> RechazadosPais()
        {
            var resultado = _context.Controles
                .GroupBy(x => x.PaisDestino)
                .Select(g => new
                {
                    Pais = g.Key,
                    Cantidad = g.Where(x => x.Estado == "Rechazado").Count()
                }).OrderByDescending(x => x.Cantidad).Take(4)
                .ToList();

            return Ok(resultado);
        }


        [HttpPost("ProductosPais")]
        public async Task<ActionResult<Controles>> ProductosPais()
        {
            var resultado = _context.Controles
                .GroupBy(x => x.PaisDestino)
                .Select(g => new
                {
                    Pais = g.Key,
                    Cantidad = g.Take(4).Count()
                }).OrderByDescending(x => x.Cantidad).Take(4)
                .ToList();

            return Ok(resultado);
        }

        [HttpPost("RechazadosMensuales")]
        public IActionResult RechazadosMensuales()
        {
            try
            {
                var datos = _context.Controles
                    .Where(c => c.Estado == "Rechazado") 
                    .GroupBy(c => c.FechaHoraPrimerControl.Month)        
                    .Select(g => new
                    {
                        Mes = g.Key,                   
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(x => x.Mes).Take(4)              
                    .ToList();

                // Retornar los datos como respuesta
                return Ok(datos);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, new { Error = "Ocurrió un error al procesar los datos.", Detalle = ex.Message });
            }
        }



        [HttpPost("ReprocesosMensuales")]
        public IActionResult ReprocesosMensuales()
        {
            try
            {
                var datos = _context.Controles
                    .Where(c => c.Estado == "Reproceso")
                    .GroupBy(c => c.FechaHoraPrimerControl.Month)
                    .Select(g => new
                    {
                        Mes = g.Key,
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(x => x.Mes).Take(4)
                    .ToList();

                // Retornar los datos como respuesta
                return Ok(datos);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, new { Error = "Ocurrió un error al procesar los datos.", Detalle = ex.Message });
            }
        }





        [HttpGet("ResumenControles")]
        public IActionResult ResumenControles()
        {
            var hoy = DateTime.Today;
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            var currentYear = DateTime.Today.Year;
            var currentMonth = DateTime.Today.Month;


            var ControlesDiarios = _context.Controles.Where(x => x.FechaHoraPrimerControl.Date == DateTime.Today).ToList().Count;
            var ControlesSemanales = _context.Controles
                .Where(x => x.FechaHoraPrimerControl.Date >= startOfWeek.Date && x.FechaHoraPrimerControl.Date < endOfWeek.Date)
                .ToList().Count;

            var ControlesMensuales = _context.Controles
                .Where(x => x.FechaHoraPrimerControl.Year == currentYear && x.FechaHoraPrimerControl.Month == currentMonth)
                .ToList().Count;




            var ControlesReprocesos = _context.Controles.Where(x => x.Estado == "Reproceso" &&
                x.FechaHoraPrimerControl.Date >= startOfWeek.Date && x.FechaHoraPrimerControl.Date < endOfWeek.Date).ToList().Count;

            var ControlesPreventivos = _context.Controles.Where(x => x.Tipodecontrol == "Preventivo" 
                 && x.FechaHoraPrimerControl.Date >= startOfWeek.Date && x.FechaHoraPrimerControl.Date < endOfWeek.Date).ToList().Count;


            var ControlesRechazados = _context.Controles.Where(x => x.Estado == "Rechazado" &&
                x.FechaHoraPrimerControl.Date >= startOfWeek.Date && x.FechaHoraPrimerControl.Date < endOfWeek.Date).ToList().Count;



            var Resultados = new {
                ControlesDiarios = ControlesDiarios,
                ControlesSemanales = ControlesSemanales,
                ControlesMensuales = ControlesMensuales,
                ControlesReprocesos = ControlesReprocesos,
                ControlesRechazados = ControlesRechazados,

                ControlesPreventivos = ControlesPreventivos

            };

            return Ok(Resultados);
        }

        [HttpGet("ComparacionControles")]
        public IActionResult ComparacionControles()
        {
            var hoy = DateTime.Today;
            var currentYear = hoy.Year;
            var currentMonth = hoy.Month;

            // Calcular el mes anterior
            var previousMonth = currentMonth - 1;
            var previousYear = currentYear;
            if (previousMonth == 0)
            {
                previousMonth = 12;
                previousYear -= 1;
            }

            // Función para calcular controles por estado y mes
            int GetControlCount(string estado, int year, int month)
            {
                return _context.Controles
                    .Where(x => x.Estado == estado && x.FechaHoraPrimerControl.Year == year && x.FechaHoraPrimerControl.Month == month)
                    .Count();
            }

            // Controles actuales
            var aprobadosActuales = GetControlCount("Aprobado", currentYear, currentMonth);
            var rechazadosActuales = GetControlCount("Rechazado", currentYear, currentMonth);
            var reprocesosActuales = GetControlCount("Reproceso", currentYear, currentMonth);

            // Controles mes anterior
            var aprobadosAnteriores = GetControlCount("Aprobado", previousYear, previousMonth);
            var rechazadosAnteriores = GetControlCount("Rechazado", previousYear, previousMonth);
            var reprocesosAnteriores = GetControlCount("Reproceso", previousYear, previousMonth);

            // Función para calcular la variación porcentual
            double CalcularVariacion(int actual, int anterior)
            {
                if (anterior == 0)
                {
                    return actual > 0 ? 100 : 0; // Si anterior es 0 y hay nuevos valores, considerar un aumento del 100%
                }
                return ((double)(actual - anterior) / anterior) * 100;
            }

            // Variaciones porcentuales
            var variacionAprobados = CalcularVariacion(aprobadosActuales, aprobadosAnteriores);
            var variacionRechazados = CalcularVariacion(rechazadosActuales, rechazadosAnteriores);
            var variacionReprocesos = CalcularVariacion(reprocesosActuales, reprocesosAnteriores);

            var resultados = new
            {
                Aprobados = new
                {
                    Actual = aprobadosActuales,
                    Anterior = aprobadosAnteriores,
                    VariacionPorcentual = variacionAprobados
                },
                Rechazados = new
                {
                    Actual = rechazadosActuales,
                    Anterior = rechazadosAnteriores,
                    VariacionPorcentual = variacionRechazados
                },
                Reprocesos = new
                {
                    Actual = reprocesosActuales,
                    Anterior = reprocesosAnteriores,
                    VariacionPorcentual = variacionReprocesos
                }
            };

            return Ok(resultados);
        }



    }
}
