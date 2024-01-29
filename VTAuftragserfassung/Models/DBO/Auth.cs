using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Models.DBO
{
    public class Auth : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; } = "vta_Auth";

        [JsonIgnore]
        public string PrimaryKeyColumn { get; set; } = "PK_Auth";

        public int PK_Auth { get; set; }
        public int FK_Vertriebsmitarbeiter { get; set; }

        [JsonIgnore]
        public string HashedAuth { get; set; } = string.Empty;
    }
}