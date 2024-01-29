using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Models.DBO
{
    public class Position : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Position";

        [JsonIgnore]
        public string PrimaryKeyColumn { get; set; } = "PK_Position";

        public int PK_Position { get; set; }
        public int FK_Artikel { get; set; }
        public int FK_Auftrag { get; set; }
        public int PositionsNummer { get; set; }
        public int Menge { get; set; }
        public double SummePosition { get; set; }
    }
}