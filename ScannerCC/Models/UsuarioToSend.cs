using System.ComponentModel.DataAnnotations;

namespace ScannerCC.Models
{
    public class UsuarioToSend
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? Email { get; set; }
        public string NombreRol { get; set; }
        public bool Activo { get; set; }
        public string? Token { get; set; }


    }
}

