using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ScannerCC.Models
{
    public class Productos
    {
        [Key]
        public int Id { get; set; }
        public int IdInformacionQuimica { get; set; }
        public string CodigoBarra { get; set; }
        public string CodigoVE { get; set; }
        public string Nombre { get; set; }
        public string URLImagen {  get; set; }
        public string? PaisDestino { get; set; }
        public int IdUsuarios { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; }
        public string Idioma { get; set; }
        public string UnidadMedida { get; set; }
        public string DescripcionCapsula { get; set; }

        // Relaciones
        public Usuarios Usuarios { get; set; }
        public InformacionQuimica InformacionQuimica { get; set; }
    }
}
