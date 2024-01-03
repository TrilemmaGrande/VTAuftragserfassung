using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models
{
    public class Position : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Position";
        public int PositionsNummer { get; set; }
        public int PK_Position { get; set; }
        public int FK_Artikel { get; set; }
        public int FK_Auftrag { get; set; }
        public int Menge { get; set; }
        public double SummePosition { get; set; }
    }
}
