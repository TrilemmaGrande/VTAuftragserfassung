﻿using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models
{
    public class Kunde : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName { get; set; } = "vta_Kunde";
        public int PK_Kunde { get; set; }
        public int FK_Gesellschafter { get; set; }
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
