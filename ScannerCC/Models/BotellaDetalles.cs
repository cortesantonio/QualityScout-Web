using System.ComponentModel.DataAnnotations;

namespace ScannerCC.Models
{
    public class BotellaDetalles
    {
        [Key]
        public int Id { get; set; }
        public string NombreBotella { get; set; }
        public int AlturaBotella { get; set; }
        public int AnchoBotella { get; set; }
    }
}
