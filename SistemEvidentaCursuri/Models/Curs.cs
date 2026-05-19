namespace SistemEvidentaCursuri.Models
{
    public class Curs
    {
        public int IdCurs { get; set; }
        public string Denumire { get; set; } = string.Empty;
        public string Formator { get; set; } = string.Empty;
        public decimal Pret { get; set; }
        public int DurataZile { get; set; }
    }
}
