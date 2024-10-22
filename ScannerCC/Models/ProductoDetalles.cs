using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ScannerCC.Models
{
    public class ProductoDetalles
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Productos")]
        public int IdProductos { get; set; }
        [ForeignKey("BotellaDetalles")]
        public int IdBotellaDetalles { get; set; }
        public int Capacidad { get; set; }
        public string TipoCapsula { get; set; }
        public string TipoEtiqueta { get; set; }
        public string ColorBotella { get; set; }
        public bool Medalla { get; set; }
        public string ColorCapsula { get; set; }
        public string TipoCorcho { get; set; }
        public int MedidaEtiquetaABoquete { get; set; }
        public int MedidaEtiquetaABase { get; set; }

        //Relaciones
        [JsonIgnore]
        public Productos Productos { get; set; }
        [JsonIgnore]
        public BotellaDetalles BotellaDetalles { get; set; }
    }
}
