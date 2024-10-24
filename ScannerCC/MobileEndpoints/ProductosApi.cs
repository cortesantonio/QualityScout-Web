using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScannerCC.Models;

namespace QualityScout.MobileEndpoints
{

    public class ProductoRecibido
    {


        public string CodigoBarra { get; set; }
        public string CodigoVE { get; set; }
        public string NombreVino { get; set; }
        public string PaisDestino { get; set; }
        public string Idioma { get; set; }
        public string UnidadMedida { get; set; }
        public string DescripcionCapsula { get; set; }

        public int? Capacidad { get; set; }
        public string? TipoCapsula { get; set; }
        public string? ColorCapsula { get; set; }
        public string? ColorBotella { get; set; }
        public string? TipoEtiqueta { get; set; }
        public string? TipoCorcho { get; set; }
        public int? MedidaEtiquetaBoquete { get; set; }
        public int? MedidaEtiquetaBase { get; set; }
        public bool? Medalla { get; set; }

        public int? IdInformacionQuimica { get; set; }
        public string? Cepa { get; set; }
        public int? AzucarMin { get; set; }
        public int? AzucarMax { get; set; }
        public int? SulfurosMin { get; set; }
        public int? SulfurosMax { get; set; }
        public int? DensidadMin { get; set; }
        public int? DensidadMax { get; set; }
        public int? GradoAlcoholicoMin { get; set; }
        public int? GradoAlcoholicoMax { get; set; }

        public int? IdBotellaDetalle { get; set; }
        public string? NombreBotella { get; set; }
        public int? AltoBotella { get; set; }
        public int? AnchoBotella { get; set; }

        public bool? GuardarHistorial { get; set; }
        public string? FechaCosecha { get; set; }
        public string? FechaProduccion { get; set; }
        public string? FechaEnvasado { get; set; }
    

}





[Route("api/[controller]")]
    [ApiController]
    public class ProductosApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosApi(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductosApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Productos>>> GetProducto()
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            return await _context.Producto.Include(x =>x.InformacionQuimica).ToListAsync();
        }

        // GET: api/ProductosApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Productos>> GetProducto(int id)
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            var producto = await _context.Producto
                 .Include(p => p.InformacionQuimica)
                 .Include(p => p.ProductoDetalles)
                     .ThenInclude(pd => pd.BotellaDetalles)
                 .Include(p => p.ProductoHistorial)
                 .Include(p => p.Usuarios)
                 .FirstOrDefaultAsync(m => m.Id == id);


            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }




        // GET: api/ProductosApi/5
        [HttpGet("GetBotella/{id}")]
        public async Task<ActionResult<Productos>> GetBotella(int id)
        {
            if (_context.BotellaDetalle == null)
            {
                return NotFound();
            }
            var botella = await _context.BotellaDetalle.Where(x => x.Id == id).FirstOrDefaultAsync();


            if (botella == null)
            {
                return NotFound();
            }

            return Ok(botella);
        }

        // GET: api/ProductosApi/5
        [HttpGet("GetBotellas")]
        public async Task<ActionResult<Productos>> GetBotellas()
        {
            if (_context.BotellaDetalle == null)
            {
                return NotFound();
            }
            var botella = await _context.BotellaDetalle.ToListAsync();


            if (botella == null)
            {
                return NotFound();
            }

            return Ok(botella);
        }

        // GET: api/ProductosApi/5
        [HttpGet("GetInfoQuimica")]
        public async Task<ActionResult<Productos>> GetInfoQuimica()
        {
           
            var infoQ = await _context.InformacionQuimica.ToListAsync();


            if (infoQ == null)
            {
                return NotFound();
            }

            return Ok(infoQ);
        }




        // PUT: api/ProductosApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateActivo/{id}")]
        public async Task<IActionResult> UpdateActivo(int id)
        {
            var p = _context.Producto.Where(x => x.Id == id).FirstOrDefault();

            if (p == null)
            {
                return NotFound();
            }

            if (p.Activo == true)
            {
                p.Activo = false;
            }
            else
            {
                p.Activo = true;
            }

            _context.Producto.Update(p);
            _context.SaveChanges();
            return Ok();


        }

        [HttpPost("CrearProducto")]
        public async Task<ActionResult<Productos>> CrearProducto([FromHeader(Name = "Authorization")] string authorization, ProductoRecibido pr)
        {
            // Extrae el token de la cabecera Authorization
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized("Token no proporcionado o no válido.");
            }

            var token = authorization.Substring("Bearer ".Length).Trim();

            var usuarioPeticion = await _context.Usuario.FirstOrDefaultAsync(u => u.Token == token);
            if (usuarioPeticion == null)
            {
                return Unauthorized("Token no válido.");
            }


            Productos producto = new Productos();

            producto.CodigoBarra = pr.CodigoBarra;
            producto.CodigoVE = pr.CodigoVE;
            producto.Nombre = pr.NombreVino;
            producto.PaisDestino = pr.PaisDestino;
            producto.Idioma = pr.Idioma;
            producto.UnidadMedida = pr.UnidadMedida;
            producto.DescripcionCapsula = pr.DescripcionCapsula;
            producto.IdUsuarios = usuarioPeticion.Id;

            // datos de temporal de prueba
            producto.URLImagen = "https://www.monteswines.com/images/botella/icons.png";


            producto.FechaRegistro = DateTime.Now;

            if (pr.IdInformacionQuimica == 0 && pr.AzucarMax != 0 && pr.SulfurosMax !=0 && pr.DensidadMax !=0 && pr.GradoAlcoholicoMax != 0 )
            {
                InformacionQuimica iq = new InformacionQuimica();
                iq.Cepa = pr.Cepa;
                iq.MinGradoAlcohol = (float)pr.GradoAlcoholicoMin;
                iq.MaxGradoAlcohol = (float)pr.GradoAlcoholicoMax;
                iq.MaxDensidad = (float)pr.DensidadMax;
                iq.MinDensidad = (float)pr.DensidadMin;
                iq.MaxSulfuroso = (float)pr.SulfurosMax;
                iq.MinSulfuroso = (float)pr.SulfurosMin;
                iq.MaxAzucar = (float)pr.AzucarMax;
                iq.MinAzucar = (float)pr.AzucarMin;

                _context.InformacionQuimica.Add(iq);
                _context.SaveChanges();

                producto.IdInformacionQuimica = iq.Id;

            }else if (pr.IdInformacionQuimica > 0)
            {
                producto.IdInformacionQuimica = (int)pr.IdInformacionQuimica;
            }

            _context.Producto.Add(producto);
            _context.SaveChanges();
            var idNuevoProducto = producto.Id;

            if(pr.GuardarHistorial == true)
            {
                ProductoHistorial productoHistorial = new ProductoHistorial();

                
                productoHistorial.FechaCosecha = DateTime.ParseExact(pr.FechaCosecha, "dd-MM-yyyy HH:mm:ss", null);
                productoHistorial.FechaProduccion = DateTime.ParseExact(pr.FechaProduccion, "dd-MM-yyyy HH:mm:ss", null);
                productoHistorial.FechaEnvasado = DateTime.ParseExact(pr.FechaEnvasado, "dd-MM-yyyy HH:mm:ss", null);
                productoHistorial.IdProductos = idNuevoProducto;
                _context.ProductoHistorial.Add(productoHistorial);
                _context.SaveChanges();

            }


            if (pr.Capacidad > 0 && pr.TipoCapsula != "") {
                BotellaDetalles botellaDetalles = new BotellaDetalles();
               

                if (pr.IdBotellaDetalle == 0 && pr.NombreBotella != "") // en el formulario de la app movil se agregara una nueva botella.
                {

                    botellaDetalles.NombreBotella = pr.NombreBotella;
                    botellaDetalles.AlturaBotella = (int)pr.AltoBotella;
                    botellaDetalles.AnchoBotella = (int)pr.AnchoBotella;

                    // recibimos los datos y se instancia el modelo.

                }
               // se instancia el modelo para crear el detalle del producto.

                ProductoDetalles productoDetalles = new ProductoDetalles();
                productoDetalles.IdProductos = producto.Id;
                productoDetalles.Capacidad = (int)pr.Capacidad;
                productoDetalles.TipoCapsula = pr.TipoCapsula;
                productoDetalles.ColorCapsula = pr.ColorCapsula;
                productoDetalles.ColorBotella = pr.ColorBotella;
                productoDetalles.TipoEtiqueta = pr.TipoEtiqueta;
                productoDetalles.TipoCorcho = pr.TipoCorcho;
                productoDetalles.MedidaEtiquetaABase = (int)pr.MedidaEtiquetaBase;
                productoDetalles.MedidaEtiquetaABoquete = (int)pr.MedidaEtiquetaBoquete;
                productoDetalles.Medalla = pr.Medalla ?? false;


                if (pr.IdBotellaDetalle > 0 ) // si se selcciono un item o botella ya registrada se setea el valor recibido
                {
                    productoDetalles.IdBotellaDetalles = (int)pr.IdBotellaDetalle;
                }
                else // si no, se registra los datos que se crearon al inicio y  se captura la id de la nueva botella, luego, se crea el detalle.
                {
                    _context.BotellaDetalle.Add(botellaDetalles);
                    _context.SaveChanges();

                    productoDetalles.IdBotellaDetalles = botellaDetalles.Id;
                }


                _context.ProductoDetalle.Add(productoDetalles);
                _context.SaveChanges();

            }



            return Ok(producto);
        }








        // DELETE: api/ProductosApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return (_context.Producto?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
