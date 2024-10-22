using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Productos Productos { get; set; }
    }
}
