using ScannerCC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScannerCC.Models
{
    public class Escaneos
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Productos")]
        public int IdProductos { get; set; }
        [ForeignKey("Usuarios")]
        public int IdUsuarios { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }

        //Relaciones
        public Productos Productos { get; set; }
        public Usuarios Usuarios { get; set; }
    }
}


