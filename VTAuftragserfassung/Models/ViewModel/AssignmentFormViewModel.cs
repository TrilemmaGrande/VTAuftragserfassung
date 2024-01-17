using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Models.ViewModel
{
    public class AssignmentFormViewModel
    {
        public Vertriebsmitarbeiter? Vertriebsmitarbeiter { get; set; } 
        public List<Artikel> Artikel { get; set; } = [];
        public List<Kunde> Kunden { get; set; } = [];
        public List<Gesellschafter> Gesellschafter { get; set; } = [];
        public int Bonus { get; set; }
        public string Notice { get; set; } = string.Empty;
    }
}
