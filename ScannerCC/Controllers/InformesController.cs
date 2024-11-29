using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ScannerCC.Models;
using System.Text;

using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Humanizer;

namespace ScannerCC.Controllers
{
    public class InformesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly string OpenAIApiKey = "";

        public InformesController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Especialista, Control de Calidad")]
        public IActionResult GestionInformes(string Busqueda)
        {
            // Filtrar informes según el criterio de búsqueda

            if (!string.IsNullOrEmpty(Busqueda))
            {
                ViewBag.Informes = _context.Informe.Where(p => p.Titulo.Contains(Busqueda) || p.Enfoque.Contains(Busqueda)).ToList();
            }
            else
            {
                ViewBag.Informes = _context.Informe.ToList();

            }

            var informe = _context.Informe
                    .Include(pd => pd.Usuarios)
                    .ToList();
            ViewBag.Informe = informe;

            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            return View();
        }

        // GET: Informes/Details
        [Authorize(Roles = "Especialista, Control de Calidad")]
        public async Task<IActionResult> Details(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.Informe == null)
            {
                return NotFound();
            }

            var informes = await _context.Informe
                .Include(i => i.Usuarios)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (informes == null)
            {
                return NotFound();
            }

            ViewBag.IdInforme = id; 
            ViewBag.titulo = informes.Titulo;
            ViewBag.enfoque = informes.Enfoque;
            ViewBag.contenido = informes.Descripcion;
            ViewBag.Fecha = informes.Fecha;
            ViewBag.nombreEncargado = informes.Usuarios.Nombre;

            return View(informes);
        }

        // GET: Informes/Create
        [Authorize(Roles = "Especialista")]
        public IActionResult Create()
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Previsualizar(string Titulo, string enfoque)
        {

            try
            {
                var jsonControles = await _context.Controles.Take(30).ToListAsync();
                string jsonString = JsonConvert.SerializeObject(jsonControles);

                // Generar el prompt
                string prompt = GeneratePrompt(jsonString, enfoque);

                // Crear el cliente HTTP
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIApiKey}");

                    // Crear la solicitud
                    var requestBody = new
                    {
                        model = "gpt-3.5-turbo", // Usa 'gpt-4' si tienes acceso
                        messages = new[]
                        {
                new { role = "system", content = "Eres un experto en control de calidad para productos de la industria de bebidas, especializado en vinos. Vas a recibir datos de control de calidad en formato JSON y debes generar un informe técnico basado en el enfoque específico que se te indique." },
                new { role = "user", content = prompt }
            }
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    // Enviar la solicitud
                    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Error en la API de OpenAI: {errorResponse}");
                    }

                    // Leer la respuesta
                    var responseData = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(responseData);
                    string message = data.choices[0].message.content;

                    // Devolver la respuesta al usuario


                    ViewBag.contenido = message;
                    ViewBag.enfoque = enfoque;
                    ViewBag.Fecha = DateTime.Now;
                    ViewBag.titulo = Titulo;


                    var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
                    ViewBag.trab = TrabajadorActivo;
                    ViewBag.nombreEncargado = TrabajadorActivo.Nombre;



                    return View(); // Devuelve una vista con el informe generado
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al generar el informe: {ex.Message}");
            }
        }
              
       
       
             private string GeneratePrompt(string json, string enfoque)
        {
            return $@"
                Voy a proporcionarte los datos de control de calidad de una botella de vino en formato JSON. 
                Quiero que generes un informe técnico enfocado en ""{enfoque}"" basado en los datos suministrados. 
                El informe debe incluir un análisis exhaustivo de los hallazgos, indicando claramente las métricas clave, 
                incluyendo porcentajes de error, cantidades y comparativas de desempeño de los diferentes productos.
                Además, se deben proporcionar conclusiones técnicas específicas basadas en los datos numéricos, 
                destacando cualquier valor significativo en el análisis. 
                No incluyas recomendaciones generales de mantenimiento; concéntrate en detalles cuantitativos para el informe. 
                El informe debe ser útil para informes de calidad o auditorías, ser conciso, no exceder las 200 palabras y 
                evitar citas textuales de los datos JSON. Utiliza términos generales pero incluye porcentajes y cantidades de ser necesario.

                JSON para el análisis: {json}.
                ";
        }



        // POST: Informes/Create
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Titulo, string Enfoque, string Descripcion)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                

                Informes informes = new Informes();
                informes.IdUsuarios = TrabajadorActivo.Id; // Asigna el Id del usuario logueado
                informes.Titulo = Titulo;
                informes.Enfoque = Enfoque;
                informes.Fecha = DateTime.Now; // Asigna la fecha actual
                informes.Descripcion = Descripcion;

                _context.Add(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("GestionInformes", "Informes");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al crear el informe: " + ex.Message);
            }
        }

        // GET: Informes/Edit
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Edit(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.Informe == null)
            {
                return NotFound();
            }

            var informes = await _context.Informe.FindAsync(id);
            if (informes == null)
            {
                return NotFound();
            }
            return View(informes);
        }

        // POST: Informes/Edit
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Titulo, string Enfoque, string Descripcion)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var informes = await _context.Informe.FindAsync(id);
                if (informes == null)
                {
                    return NotFound("Informe no encontrado.");
                }

                informes.Titulo = Titulo;
                informes.Enfoque = Enfoque;
                informes.Descripcion = Descripcion;

                _context.Update(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("GestionInformes", "Informes");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al editar el informe: " + ex.Message);
            }
        }


        // GET: Informes/Delete
        [Authorize(Roles = "Especialista")]
        public async Task<IActionResult> Delete(int? id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            if (id == null || _context.Informe == null)
            {
                return NotFound();
            }

            var informes = await _context.Informe
                .Include(i => i.Usuarios)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (informes == null)
            {
                return NotFound();
            }

            ViewBag.IdInforme = id;
            ViewBag.titulo = informes.Titulo;
            ViewBag.enfoque = informes.Enfoque;
            ViewBag.contenido = informes.Descripcion;
            ViewBag.Fecha = informes.Fecha;
            ViewBag.nombreEncargado = informes.Usuarios.Nombre;

            return View(informes);
        }

        // POST: Informes/Delete
        [Authorize(Roles = "Especialista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var TrabajadorActivo = _context.Usuario.Where(t => t.Rut.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.trab = TrabajadorActivo;

            try
            {
                var informes = await _context.Informe.FindAsync(id);
                if (informes == null)
                {
                    return NotFound("Informe no encontrado.");
                }

                _context.Informe.Remove(informes);
                await _context.SaveChangesAsync();

                return RedirectToAction("GestionInformes", "Informes");
            }
            catch (Exception ex)
            {
                return Problem("Ocurrió un error al eliminar el informe: " + ex.Message);
            }
        }

        private bool InfExists(int id)
        {
            return (_context.Informe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
