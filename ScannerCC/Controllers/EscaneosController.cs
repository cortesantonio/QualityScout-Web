using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace ScannerCC.Controllers
{
    public class EscaneosController : Controller
    {
        private readonly AppDbContext _context;

        public EscaneosController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Escaneos/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Escaneo == null)
            {
                return NotFound();
            }

            var escaneo = await _context.Escaneo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (escaneo == null)
            {
                return NotFound();
            }

            return View(escaneo);
        }
    }
}
