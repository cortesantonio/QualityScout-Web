namespace QualityScout.Models
{
    public class InfoViewModel
    {
        public int ControlesDiarios { get; set; }
        public int ControlesSemanales { get; set; }
        public int ControlesAprobados { get; set; }
        public int ControlesReprocesados { get; set; }
        public int ControlesRechazados { get; set; }
        public int ControlesPreventivos { get; set; }
        public string AprobadosMesAntiguo { get; set; }
        public string ReprocesadosMesAntiguo { get; set; }
        public string RechazadosMesAntiguo { get; set; }
        public string AprobadosMesActual { get; set; }
        public string ReprocesadosMesActual { get; set; }
        public string RechazadosMesActual { get; set; }
        public string VariacionAprobados { get; set; } 
        public string VariacionReprocesados { get; set; } 
        public string VariacionRechazados { get; set; }
        public string MesAnterior { get; set; }
        public int AnioAnterior { get; set; }
    }
}
