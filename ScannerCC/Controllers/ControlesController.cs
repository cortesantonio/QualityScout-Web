using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityScout.Models;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class ControlesController : Controller
    {
        private readonly AppDbContext _context;

        public ControlesController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Especialista, Control de Calidad")]
        public IActionResult GestionControles(string orderByDate)
        {
            var trabajadorActivo = _context.Usuario.FirstOrDefault(t => t.Rut.Equals(User.Identity.Name));
            ViewBag.Trab = trabajadorActivo;

            // Obtén la lista de controles y ordénala según el valor de orderByDate
            var controles = _context.Controles.AsQueryable();

            if (orderByDate == "asc")
            {
                controles = controles.OrderBy(c => c.FechaHoraPrimerControl);
            }
            else if (orderByDate == "desc")
            {
                controles = controles.OrderByDescending(c => c.FechaHoraPrimerControl);
            }

            ViewBag.Controles = controles.ToList();
            ViewBag.Productos = _context.Producto.ToList();

            return View();
        }

        [Authorize(Roles = "Especialista, Control de Calidad")]
        public IActionResult Controles()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Comienza la semana el lunes
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Calcular controles diarios
            var controlesDiarios = _context.Controles
                .Where(c => c.FechaHoraPrimerControl.Date == today)
                .Count();

            // Calcular controles semanales
            var controlesSemanales = _context.Controles
                .Where(c => c.FechaHoraPrimerControl.Date >= startOfWeek && c.FechaHoraPrimerControl.Date <= today)
                .Count();

            // Calcular controles rechazados
            var controlesRechazados = _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Rechazado"))
                .Count();

            // Calcular controles preventivos
            var controlesPreventivos = _context.Controles
                .Where(c => c.Tipodecontrol != null && c.Tipodecontrol.Contains("Preventivo"))
                .Count();

            // Calcular controles reprocesados
            var controlesReprocesados = _context.Controles
                .Where(c => c.Estado != null && c.Estado.Contains("Reproceso"))
                .Count();

            // Manejo de nulos: Asignar valor 0 si no hay datos
            var model = new InfoViewModel
            {
                ControlesDiarios = controlesDiarios > 0 ? controlesDiarios : 0,
                ControlesSemanales = controlesSemanales > 0 ? controlesSemanales : 0,
                ControlesRechazados = controlesRechazados > 0 ? controlesRechazados : 0,
                ControlesPreventivos = controlesPreventivos > 0 ? controlesPreventivos : 0,
                ControlesReprocesados = controlesReprocesados > 0 ? controlesReprocesados : 0
            };

            // Procesar datos para el gráfico de exportaciones por país 
            var datosGrafico = _context.Controles
                .Where(c => c.PaisDestino != null)
                .GroupBy(c => new { c.PaisDestino, Mes = c.FechaHoraPrimerControl.Month })
                .Select(g => new
                {
                    PaisDestino = g.Key.PaisDestino,
                    Mes = g.Key.Mes,
                    Cantidad = g.Count()
                })
                .ToList();
            var paises = datosGrafico.Select(d => d.PaisDestino).Distinct().ToList();
            var mesesNumericos = datosGrafico.Select(d => d.Mes).Distinct().OrderBy(m => m).ToList();
            var mesesTexto = mesesNumericos
                .Select(m => new DateTime(1, m, 1).ToString("MMMM"))
                .Select(nombreMes => char.ToUpper(nombreMes[0]) + nombreMes.Substring(1))
                .ToList();
            var exportacionesPorMes = mesesNumericos.Select(mes =>
                datosGrafico
                    .Where(d => d.Mes == mes)
                    .OrderBy(d => d.PaisDestino)
                    .Select(d => d.Cantidad)
                    .ToList()
            ).ToList();
            ViewBag.Paises = paises;
            ViewBag.Meses = mesesTexto;
            ViewBag.Exportaciones = exportacionesPorMes;

            return View(model);
        }

        [HttpGet]
        public IActionResult Reconocimiento(int Producto)
        {
            if (Producto != 0)
            {
                ViewBag.Producto = _context.Producto.Where(x => x.Id == Producto).FirstOrDefault();
                return View();
            }

            return BadRequest("Producto no válido.");
        }

        // GET: Controles
        [Authorize(Roles = "Control de Calidad")]
        public async Task<IActionResult> Index()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            var appDbContext = _context.Controles.Include(c => c.Productos);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Controles/Details
        [Authorize(Roles = "Especialista, Control de Calidad")]
        public async Task<IActionResult> Details2(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.Controles == null)
            {
                return NotFound();
            }

            var controles = await _context.Controles
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (controles == null)
            {
                return NotFound();
            }
            return View(controles);
        }

        // GET: Controles/Create
        [Authorize(Roles = "Control de Calidad")]
        public IActionResult CreateControl(int? idProducto)
        {
            var TrabajadorActivo = _context.Usuario
                .FirstOrDefault(t => t.Rut.Equals(User.Identity.Name));
            ViewBag.trab = TrabajadorActivo;

            var productos = _context.Producto.Select(p => new { p.Id, p.Nombre }).ToList();


            ViewBag.IdProductos = new SelectList(productos, "Id", "Nombre", idProducto);
            ViewBag.PaisDestino = _context.Producto.Where(x => x.Id == idProducto).FirstOrDefault().PaisDestino;
            ViewBag.IdProducto = idProducto;

            return View();
        }



        // POST: Controles/Create
        [Authorize(Roles = "Control de Calidad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateControl(int IdProductos, string Linea, string PaisDestino, string Comentario, string Tipodecontrol, string Estado)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                // Obtener el usuario logueado
                var currentUser = await _context.Usuario.FirstOrDefaultAsync(u => u.Rut == User.Identity.Name);
                if (currentUser == null)
                {
                    return Problem("Usuario no encontrado.");
                }

                Controles control = new Controles();
                control.IdProductos = IdProductos; 
                control.Linea = Linea;
                control.PaisDestino = PaisDestino;
                control.Comentario = Comentario;
                control.Tipodecontrol = Tipodecontrol;
                control.Estado = Estado;
                control.IdUsuarios = currentUser.Id; // Asignar IdUsuarios
                control.FechaHoraPrimerControl = DateTime.Now; // Asignar fecha actual

                _context.Add(control);
                await _context.SaveChangesAsync();
                return RedirectToAction("GestionControles", "Controles");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el control: " + ex.Message);
            }
        }

        // GET: Controles/Edit
        [Authorize(Roles = "Control de Calidad")]
        public async Task<IActionResult> Edit2(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var control = await _context.Controles
                    .Include(c => c.Productos) 
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (control == null)
                {
                    return NotFound("Control no encontrado.");
                }

                // Verificar si el estado es "Reproceso"
                if (control.Estado != "Reproceso")
                {
                    return RedirectToAction("GestionControles", "Controles");
                }

                return View(control); // Enviar el control a la vista
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al cargar el control para edición: " + ex.Message);
            }
        }

        // POST: Controles/Edit
        [Authorize(Roles = "Control de Calidad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(int id, string Comentario, string EstadoFinal)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id != id) 
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Controles control = new Controles(); 
                    control = await _context.Controles.FindAsync(id);
                    if (control == null)
                    {
                        return NotFound("Control no encontrado.");
                    }

                    control.Comentario = Comentario;
                    control.EstadoFinal = EstadoFinal;
                    control.FechaHoraControlFinal = DateTime.Now; // Asignar fecha actual al finalizar el control

                    // Guardar cambios
                    _context.Update(control);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("GestionControles", "Controles");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ControlExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    return Problem("Ocurrió un error al editar el control: " + ex.Message);
                }
            }

            return View();
        }


        // GET: Controles/Delete
        [Authorize(Roles = "Control de Calidad")]
        public async Task<IActionResult> Delete2(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.Controles == null)
            {
                return NotFound();
            }

            var controles = await _context.Controles
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (controles == null)
            {
                return NotFound();
            }

            return View(controles);
        }

        // POST: Controles/Delete
        [Authorize(Roles = "Control de Calidad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var control = await _context.Controles.FindAsync(id);
                if (control == null)
                {
                    return NotFound("Control no encontrado.");
                }

                _context.Controles.Remove(control);
                await _context.SaveChangesAsync();
                return RedirectToAction("GestionControles", "Controles");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar el control: " + ex.Message);
            }
        }

        private bool ControlExists(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            return _context.Controles.Any(e => e.Id == id);
        }

        [HttpGet]
		[Route("Productos/getData")]
		public IActionResult GetInfo()
		{
			var data = _context.Producto.ToList();

			return Json(data);
		}
	}


}
