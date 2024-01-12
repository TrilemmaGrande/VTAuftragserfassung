using System.Text.Json.Serialization;
using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Models.ViewModel
{
    public class AssignmentViewModel : IDatabaseObject
    {
        [JsonIgnore]
        public string TableName => "vta_Auftrag";
        public List<PositionViewModel>? PositionenVM { get; set; } = [];
        public Auftrag? Auftrag { get; set; }
        public Kunde? Kunde { get; set; }
        public Gesellschafter? Gesellschafter { get; set; }
    }
}
