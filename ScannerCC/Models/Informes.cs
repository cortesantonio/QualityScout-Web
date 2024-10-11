using System.ComponentModel.DataAnnotations;

namespace ScannerCC.Models
{
    public class Informes
    {
        [Key]
        public int Id { get; set; }
        public int IdUsuarios { get; set; }
        public string Titulo { get; set; }
        public string Enfoque { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }

        // Relaciones
        public Usuarios Usuarios { get; set; }
    }
}
