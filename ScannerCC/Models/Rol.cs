using System.ComponentModel.DataAnnotations;

namespace ScannerCC.Models
{
    public class Rol
    {
        [Key]
        public int idRol { get; set; }
        public string? Nombre { get; set; }
        
    }
}

