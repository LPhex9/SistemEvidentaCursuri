namespace SistemEvidentaCursuri.Models
{
    public class Cursant
    {
        public int IdCursant { get; set; }
        public string Nume { get; set; } = string.Empty;
        public string Prenume { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string NumeComplet => $"{Nume} {Prenume}";
    }
}
