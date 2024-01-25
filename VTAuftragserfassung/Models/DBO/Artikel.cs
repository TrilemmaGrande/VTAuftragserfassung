using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Models.DBO
{
    public class Artikel : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Artikel";

        [JsonIgnore]
        public string PrimaryKeyColumn { get; set; } = "PK_Artikel";

        public int PK_Artikel { get; set; }
        public string Artikelnummer { get; set; } = string.Empty;
        public string Bezeichnung1 { get; set; } = string.Empty;
        public string Bezeichnung2 { get; set; } = string.Empty;
        public double Preis { get; set; }
        public string Verpackungseinheit { get; set; } = string.Empty;
    }
}