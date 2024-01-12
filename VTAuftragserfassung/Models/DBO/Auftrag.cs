using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models.Enum;

namespace VTAuftragserfassung.Models.DBO
{
    public class Auftrag : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Auftrag";
        public int PK_Auftrag { get; set; }
        public int FK_Kunde { get; set; }
        public int FK_Vertriebsmitarbeiter { get; set; }
        public double SummeAuftrag { get; set; }
        public Auftragsstatus Auftragsstatus { get; set; } = Auftragsstatus.Erfasst;
        public int HatZugabe { get; set; }
        public string Hinweis { get; set; } = string.Empty;
        public DateTime ErstelltAm { get; set; } = DateTime.Now;
        public DateTime LetzteStatusAenderung { get; set; } = DateTime.Now;
    }
}
