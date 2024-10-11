using System.ComponentModel.DataAnnotations;

namespace ScannerCC.Models
{
    public class InformacionQuimica
    {
        [Key]
        public int Id { get; set; }
        public string Cepa { get; set; }
        public float MinAzucar { get; set; }
        public float MaxAzucar { get; set; }
        public float MinSulfuroso { get; set; }
        public float MaxSulfuroso { get; set; }
        public float MinDensidad { get; set; }
        public float MaxDensidad { get; set; }
        public float MinGradoAlcohol { get; set; }
        public float MaxGradoAlcohol { get; set; }
    }
}
