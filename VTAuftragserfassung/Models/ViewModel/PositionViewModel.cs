using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Models.ViewModel
{
    public class PositionViewModel : IDatabaseObject
    {
        public string TableName => "vta_Position";
        public Position? Position { get; set; }
        public Artikel? Artikel { get; set; }
    }
}
