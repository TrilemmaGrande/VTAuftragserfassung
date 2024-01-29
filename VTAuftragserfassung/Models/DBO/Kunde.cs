using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Models.DBO
{
    public class Kunde : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Kunde";

        [JsonIgnore]
        public string PrimaryKeyColumn { get; set; } = "PK_Kunde";

        public int PK_Kunde { get; set; }
        public int FK_Gesellschafter { get; set; }
        public string Kundennummer { get; set; } = string.Empty;
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public int IstWerkstatt { get; set; }
        public int IstHandel { get; set; }
        public string Firma { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Strasse { get; set; } = string.Empty;
        public string Postleitzahl { get; set; } = string.Empty;
        public string Ort { get; set; } = string.Empty;
    }
}