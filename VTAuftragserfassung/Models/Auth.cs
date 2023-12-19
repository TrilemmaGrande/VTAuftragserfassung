﻿using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models
{
    public class Auth : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; } = "vta_Auth";
        public int PK_Auth { get; set; }
        public int FK_Vertriebsmitarbeiter { get; set; }
        [JsonIgnore]
        public string HashedAuth { get; set; } = string.Empty;
    }
}
