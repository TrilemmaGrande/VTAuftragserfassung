using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Models.DBO
{
    public class Gesellschafter : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Gesellschafter";

        [JsonIgnore]
        public string PrimaryKeyColumn { get; set; } = "PK_Gesellschafter";

        public int PK_Gesellschafter { get; set; }
        public string Firmenname { get; set; } = string.Empty;
    }
}