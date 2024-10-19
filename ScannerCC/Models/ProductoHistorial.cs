using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScannerCC.Models
{
    public class ProductoHistorial
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Productos")]
        public int IdProductos { get; set; }
        public DateTime FechaCosecha { get; set; }
        public DateTime FechaProduccion { get; set; }
        public DateTime FechaEnvasado { get; set; }

        //Relaciones
        public Productos Productos { get; set; }
    }
}
