using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models
{
    public class Gesellschafter : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Gesellschafter";
        public int PK_Gesellschafter { get; set; }
        public string Firmenname { get; set; } = string.Empty;
    }
}
