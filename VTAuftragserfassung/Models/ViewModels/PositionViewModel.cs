using VTAuftragserfassung.Database.DataAccess;

namespace VTAuftragserfassung.Models.ViewModels
{
    public class PositionViewModel : IDatabaseObject
    {
        public string TableName => "vta_Position";
        public Position? Position { get; set; }
        public Artikel? Artikel { get; set; }
    }
}
