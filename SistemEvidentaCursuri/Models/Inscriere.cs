namespace SistemEvidentaCursuri.Models
{
    public class Inscriere
    {
        public int IdInscriere { get; set; }
        public int IdCursant { get; set; }
        public int IdCurs { get; set; }
        public DateTime DataInscriere { get; set; }
        public string StatusPlata { get; set; } = "Neachitat";

        // Navigation (for display)
        public string NumeCursant { get; set; } = string.Empty;
        public string DenumireCurs { get; set; } = string.Empty;
        public decimal PretCurs { get; set; }
    }
}
