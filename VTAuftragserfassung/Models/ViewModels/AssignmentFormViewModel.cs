using System.Text.Json.Serialization;

namespace VTAuftragserfassung.Models.ViewModels
{
    public class AssignmentFormViewModel
    {
        [JsonIgnore]
        public string TableName => "vta_Auftrag";
        public Vertriebsmitarbeiter? Vertriebsmitarbeiter { get; set; }
        public List<Artikel> Artikel { get; set; } = [];
        public List<Kunde> Kunden { get; set; } = [];
        public List<Gesellschafter> Gesellschafter { get; set; } = [];
    }
}
