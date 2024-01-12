using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models.DBO
{
    public class Vertriebsmitarbeiter : IDatabaseObject
    {
        public string TableName { get; } = "vta_Vertriebsmitarbeiter";
        public int PK_Vertriebsmitarbeiter { get; set; }
        public string MitarbeiterId { get; set; } = string.Empty;
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public string Firma { get; set; } = string.Empty;
    }
}
