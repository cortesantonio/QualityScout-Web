namespace QualityScout.Models
{
    public class UserToReceive
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? Email { get; set; }
        public int RolId { get; set; }

        public string Password { get; set; }

    }
}
