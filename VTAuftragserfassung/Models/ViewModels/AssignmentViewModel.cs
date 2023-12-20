using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models.ViewModels
{
    public class AssignmentViewModel : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName => "vta_Auftrag";
        public List<PositionViewModel>? PositionenVM { get; set; }
        public Auftrag? Auftrag { get; set; }
        public Kunde? Kunde { get; set; }
        public Gesellschafter? Gesellschafter { get; set; }

    }
}
