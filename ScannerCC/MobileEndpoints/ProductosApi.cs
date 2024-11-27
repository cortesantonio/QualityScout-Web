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
        public string? UrlImagen { get; set; }


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
        [HttpGet("GetProductoToEdit/{id}")]
        public async Task<ActionResult<Productos>> GetProductoToEdit(int id)
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            var producto = await _context.Producto
                 .Include(p => p.InformacionQuimica)
                 .Include(p => p.ProductoHistorial)
                 .FirstOrDefaultAsync(m => m.Id == id);
            var detalles = await _context.ProductoDetalle.Include(x=>x.BotellaDetalles).Where(x => x.IdProductos == id).FirstOrDefaultAsync();
            var historial = await _context.ProductoHistorial.Where(x => x.IdProductos == id).FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound();
            }


            var productoRecibido = new ProductoRecibido();

            productoRecibido.CodigoBarra = producto.CodigoBarra;
            productoRecibido.CodigoVE = producto.CodigoVE;
            productoRecibido.NombreVino = producto.Nombre;
            productoRecibido.PaisDestino = producto.PaisDestino;
            productoRecibido.Idioma = producto.Idioma;
            productoRecibido.UnidadMedida = producto.UnidadMedida;
            productoRecibido.DescripcionCapsula = producto.DescripcionCapsula;

            if(detalles != null)
            {
                productoRecibido.Capacidad = detalles.Capacidad;
                productoRecibido.TipoCapsula = detalles.TipoCapsula;
                productoRecibido.ColorCapsula = detalles.ColorCapsula;
                productoRecibido.ColorBotella = detalles.ColorBotella;
                productoRecibido.TipoEtiqueta = detalles.TipoEtiqueta;
                productoRecibido.TipoCorcho = detalles.TipoCorcho;
                productoRecibido.MedidaEtiquetaBoquete = detalles.MedidaEtiquetaABoquete;
                productoRecibido.MedidaEtiquetaBase = detalles.MedidaEtiquetaABase;
                productoRecibido.Medalla = detalles.Medalla;
                if (detalles.BotellaDetalles != null)
                {
                    productoRecibido.IdBotellaDetalle = detalles.BotellaDetalles.Id;
                    productoRecibido.NombreBotella = detalles.BotellaDetalles.NombreBotella;
                    productoRecibido.AltoBotella = detalles.BotellaDetalles.AlturaBotella;
                    productoRecibido.AnchoBotella = detalles.BotellaDetalles.AnchoBotella;
                }
            }


            productoRecibido.IdInformacionQuimica = producto.IdInformacionQuimica;
            productoRecibido.Cepa = producto.InformacionQuimica?.Cepa;
            productoRecibido.AzucarMin = (int?)producto.InformacionQuimica?.MinAzucar;
            productoRecibido.AzucarMax = (int?)producto.InformacionQuimica?.MaxAzucar;
            productoRecibido.SulfurosMin = (int?)producto.InformacionQuimica?.MinSulfuroso;
            productoRecibido.SulfurosMax = (int?)producto.InformacionQuimica?.MaxSulfuroso;
            productoRecibido.DensidadMin = (int?)producto.InformacionQuimica?.MinDensidad;
            productoRecibido.DensidadMax = (int?)producto.InformacionQuimica?.MaxDensidad;
            productoRecibido.GradoAlcoholicoMin = (int?)producto.InformacionQuimica?.MinGradoAlcohol;
            productoRecibido.GradoAlcoholicoMax = (int?)producto.InformacionQuimica?.MaxGradoAlcohol;

            
            if(historial !=null)
            {
                productoRecibido.GuardarHistorial = producto.ProductoHistorial?.Any();
                productoRecibido.FechaCosecha = producto.ProductoHistorial?.FirstOrDefault()?.FechaCosecha.ToString("dd-MM-yyyy HH:mm:ss");
                productoRecibido.FechaProduccion = producto.ProductoHistorial?.FirstOrDefault()?.FechaProduccion.ToString("dd-MM-yyyy HH:mm:ss");
                productoRecibido.FechaEnvasado = producto.ProductoHistorial?.FirstOrDefault()?.FechaEnvasado.ToString("dd-MM-yyyy HH:mm:ss");

            }
      
            return Ok(productoRecibido);
        }

        [HttpPost("EditarProducto/{IdOriginalProducto}")]
        public async Task<IActionResult> EditarProducto(ProductoRecibido productoEditado, int IdOriginalProducto)
        {
            if (_context.Producto == null)
            {
                return NotFound();
            }
            var productoOriginal = await _context.Producto
                 .Include(p => p.InformacionQuimica)
                 .Include(p => p.ProductoHistorial)
                 .FirstOrDefaultAsync(m => m.Id == IdOriginalProducto);
            var detalles = await _context.ProductoDetalle.Include(x => x.BotellaDetalles).Where(x => x.IdProductos == IdOriginalProducto).FirstOrDefaultAsync();
            var historial = await _context.ProductoHistorial.Where(x => x.IdProductos == IdOriginalProducto).FirstOrDefaultAsync();

            if (productoOriginal == null)
            {
                return NotFound();
            }

            if (productoOriginal.CodigoBarra != productoEditado.CodigoBarra)
            {
                productoOriginal.CodigoBarra = productoEditado.CodigoBarra;
            }
            if (productoOriginal.CodigoVE != productoEditado.CodigoVE)
            {
                productoOriginal.CodigoVE = productoEditado.CodigoVE;
            }
            if (productoOriginal.Nombre != productoEditado.NombreVino)
            {
                productoOriginal.Nombre = productoEditado.NombreVino;
            }
            if (productoOriginal.PaisDestino != productoEditado.PaisDestino)
            {
                productoOriginal.PaisDestino = productoEditado.PaisDestino;
            }
            if (productoOriginal.Idioma != productoEditado.Idioma)
            {
                productoOriginal.Idioma = productoEditado.Idioma;
            }
            if (productoOriginal.UnidadMedida != productoEditado.UnidadMedida)
            {
                productoOriginal.UnidadMedida = productoEditado.UnidadMedida;
            }
            if (productoOriginal.DescripcionCapsula != productoEditado.DescripcionCapsula)
            {
                productoOriginal.DescripcionCapsula = productoEditado.DescripcionCapsula;
            }
            if (productoOriginal.IdInformacionQuimica != productoEditado.IdInformacionQuimica)
            {
                productoOriginal.IdInformacionQuimica = (int)productoEditado.IdInformacionQuimica;
            }


            if(detalles != null)
            {

                // Comparación para detalles
                if ( productoEditado.Capacidad.HasValue && detalles.Capacidad != productoEditado.Capacidad)
                {
                    detalles.Capacidad = productoEditado.Capacidad.Value;
                }
                if ( detalles.TipoCapsula != productoEditado.TipoCapsula)
                {
                    detalles.TipoCapsula = productoEditado.TipoCapsula;
                }
                if ( detalles.ColorCapsula != productoEditado.ColorCapsula)
                {
                    detalles.ColorCapsula = productoEditado.ColorCapsula;
                }
                if ( detalles.ColorBotella != productoEditado.ColorBotella)
                {
                    detalles.ColorBotella = productoEditado.ColorBotella;
                }
                if ( detalles.TipoEtiqueta != productoEditado.TipoEtiqueta)
                {
                    detalles.TipoEtiqueta = productoEditado.TipoEtiqueta;
                }
                if ( detalles.TipoCorcho != productoEditado.TipoCorcho)
                {
                    detalles.TipoCorcho = productoEditado.TipoCorcho;
                }
                if ( productoEditado.MedidaEtiquetaBoquete.HasValue && detalles.MedidaEtiquetaABoquete != productoEditado.MedidaEtiquetaBoquete)
                {
                    detalles.MedidaEtiquetaABoquete = productoEditado.MedidaEtiquetaBoquete.Value;
                }
                if ( productoEditado.MedidaEtiquetaBase.HasValue && detalles.MedidaEtiquetaABase != productoEditado.MedidaEtiquetaBase)
                {
                    detalles.MedidaEtiquetaABase = productoEditado.MedidaEtiquetaBase.Value;
                }
                if ( productoEditado.Medalla.HasValue && detalles.Medalla != productoEditado.Medalla)
                {
                    detalles.Medalla = productoEditado.Medalla.Value;
                }
                if ( productoEditado.IdBotellaDetalle.HasValue && detalles.IdBotellaDetalles != productoEditado.IdBotellaDetalle)
                {
                    detalles.IdBotellaDetalles = (int)productoEditado.IdBotellaDetalle;

                }

            }

            if(detalles == null && productoEditado.Capacidad != null)
            {
                ProductoDetalles nuevoDetalles = new ProductoDetalles();
                nuevoDetalles.IdProductos = IdOriginalProducto;
                nuevoDetalles.IdBotellaDetalles = (int)productoEditado.IdBotellaDetalle;
                nuevoDetalles.Capacidad = (int)productoEditado.Capacidad;
                nuevoDetalles.TipoCapsula = productoEditado.TipoCapsula;
                nuevoDetalles.TipoEtiqueta = productoEditado.TipoEtiqueta;
                nuevoDetalles.ColorBotella = productoEditado.ColorBotella;
                nuevoDetalles.Medalla = (bool)productoEditado.Medalla;
                nuevoDetalles.ColorCapsula = productoEditado.ColorCapsula;
                nuevoDetalles.TipoCorcho = productoEditado.TipoCorcho;
                nuevoDetalles.MedidaEtiquetaABase = (int)productoEditado.MedidaEtiquetaBase;
                nuevoDetalles.MedidaEtiquetaABoquete = (int)productoEditado.MedidaEtiquetaBoquete;

                _context.ProductoDetalle.Add(nuevoDetalles);
                _context.SaveChanges();

            }



            // Comparación para ProductoHistorial
            if (historial != null)
            {
                // Comparación y actualización de FechaCosecha
                if (!string.IsNullOrEmpty(productoEditado.FechaCosecha) &&
                    historial.FechaCosecha.ToString("dd-MM-yyyy HH:mm:ss") != productoEditado.FechaCosecha)
                {
                    historial.FechaCosecha = DateTime.ParseExact(productoEditado.FechaCosecha, "dd-MM-yyyy HH:mm:ss", null);
                }

                // Comparación y actualización de FechaProduccion
                if (!string.IsNullOrEmpty(productoEditado.FechaProduccion) &&
                    historial.FechaProduccion.ToString("dd-MM-yyyy HH:mm:ss") != productoEditado.FechaProduccion)
                {
                    historial.FechaProduccion = DateTime.ParseExact(productoEditado.FechaProduccion, "dd-MM-yyyy HH:mm:ss", null);
                }

                // Comparación y actualización de FechaEnvasado
                if (!string.IsNullOrEmpty(productoEditado.FechaEnvasado) &&
                    historial.FechaEnvasado.ToString("dd-MM-yyyy HH:mm:ss") != productoEditado.FechaEnvasado)
                {
                    historial.FechaEnvasado = DateTime.ParseExact(productoEditado.FechaEnvasado, "dd-MM-yyyy HH:mm:ss", null);
                }
            }
            if (historial == null &&
                (!string.IsNullOrEmpty(productoEditado.FechaCosecha) ||
                 !string.IsNullOrEmpty(productoEditado.FechaProduccion) ||
                 !string.IsNullOrEmpty(productoEditado.FechaEnvasado)))
            {
                ProductoHistorial nuevoHistorial = new ProductoHistorial
                {
                    IdProductos = IdOriginalProducto,
                    FechaCosecha = (DateTime)(!string.IsNullOrEmpty(productoEditado.FechaCosecha)
                        ? DateTime.ParseExact(productoEditado.FechaCosecha, "dd-MM-yyyy HH:mm:ss", null)
                        : (DateTime?)null),
                    FechaProduccion = (DateTime)(!string.IsNullOrEmpty(productoEditado.FechaProduccion)
                        ? DateTime.ParseExact(productoEditado.FechaProduccion, "dd-MM-yyyy HH:mm:ss", null)
                        : (DateTime?)null),
                    FechaEnvasado = (DateTime)(!string.IsNullOrEmpty(productoEditado.FechaEnvasado)
                        ? DateTime.ParseExact(productoEditado.FechaEnvasado, "dd-MM-yyyy HH:mm:ss", null)
                        : (DateTime?)null)
                };

                _context.ProductoHistorial.Add(nuevoHistorial);
                _context.SaveChanges();
            }







            _context.Producto.Update(productoOriginal);
            _context.SaveChanges();

            if (detalles != null)
            {
                _context.ProductoDetalle.Update(detalles);
                _context.SaveChanges();
            }
            if (historial != null)
            {
                _context.ProductoHistorial.Update(historial);
                _context.SaveChanges();
            }
            
            return Ok();

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

            if (ProductoExists(pr.CodigoBarra))
            {
                return BadRequest("El codigo de barra ya existe en base de datos");
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
            producto.URLImagen = pr.UrlImagen;

            // datos de temporal de prueba


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



        [HttpPost("EstaRegistrado")]
        public async Task<ActionResult> EstaRegistrado([FromHeader(Name = "Authorization")] string authorization, string codigo)
        {
            // Validar el token en la cabecera
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized("Token no válido o ausente.");
            }

            string token = authorization.Substring("Bearer ".Length).Trim();
            var usuarioPeticion = await _context.Usuario.FirstOrDefaultAsync(u => u.Token == token);
            if (usuarioPeticion == null)
            {
                return Unauthorized("Token no válido.");
            }

            // Obtener el código desde el cuerpo de la solicitud
            if (string.IsNullOrEmpty(codigo))
            {
                return BadRequest("El código no puede estar vacío.");
            }

            // Buscar el producto en la base de datos
            var producto = await _context.Producto.FirstOrDefaultAsync(p => p.CodigoBarra == codigo);

            if (producto == null)
            {
                return NotFound(new { mensaje = "El producto no está registrado." });
            }


            DateTime fechaHoy = DateTime.Now;


            Escaneos E = new Escaneos();
            E.IdProductos = producto.Id;
            E.IdUsuarios = usuarioPeticion.Id;
            E.Fecha = fechaHoy.Date;
            E.Hora = fechaHoy.TimeOfDay;
            _context.Escaneo.Add(E);
            _context.SaveChanges();

            // Devolver la información del producto si existe
            return Ok(new
            {
                id = producto.Id,
                codigoBarra = producto.CodigoBarra,
            });
        }





        private bool ProductoExists(string codigoBarra)
        {
            return (_context.Producto?.Any(e => e.CodigoBarra == codigoBarra)).GetValueOrDefault();
        }
    }
}
