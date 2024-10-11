using System.ComponentModel.DataAnnotations;

namespace ScannerCC.Models
{
    public class Usuarios    
    {
        [Key]
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? Email { get; set; }
        public int RolId { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public bool Activo { get; set; }
        public string? Token { get; set; }

        //Relaciones
        public Rol Rol { get; set; }

    }
}

