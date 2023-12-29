﻿using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models.ViewModels
{
    public class AssignmentFormViewModel : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName => "vta_Auftrag";
        public Vertriebsmitarbeiter? Vertriebsmitarbeiter { get; set; } 
        public List<Artikel> Artikel { get; set; } = [];
        public List<Kunde> Kunden { get; set; } = [];
        public List<Gesellschafter> Gesellschafter { get; set; } = [];
        public int Bonus { get; set; }
        public string Notice { get; set; } = string.Empty;
    }
}
