using System.ComponentModel.DataAnnotations;
using System.Drawing;


namespace ScannerCC.Models
{
    public class Controles
    {
        [Key]
        public int Id { get; set; }
        public int IdProductos { get; set; }
        public string Linea { get; set; }
        public string? PaisDestino { get; set; }
        public string? Comentario { get; set; }
        public string? Tipodecontrol { get; set; }
        public DateTime FechaHoraPrimerControl { get; set; }
        public string Estado { get; set; }
        public string? EstadoFinal { get; set; }
        public int IdUsuarios { get; set; }
        public DateTime? FechaHoraControlFinal { get; set; }

        public Productos Productos { get; set; }
        public Usuarios Usuarios { get; set; }
    }
}
