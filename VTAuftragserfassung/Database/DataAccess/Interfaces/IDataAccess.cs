using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;

namespace VTAuftragserfassung.Database.DataAccess
{
    public interface IDataAccess<IDatabaseObject>
    {
        #region Public Methods

        int CountAssignmentsByUserId(string userId);

        int Create<T>(T? dbModel) where T : Interfaces.IDatabaseObject;

        void CreateAll<T>(List<T>? dbModels) where T : Interfaces.IDatabaseObject;

        public List<T>? ReadAll<T>(T? dbModel) where T : IDatabaseObject;

        List<Auftrag>? ReadAssignmentsPaginatedByUserId(string userId, Pagination? pagination);

        T1? ReadObjectByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
            where T1 : Interfaces.IDatabaseObject
            where T2 : Interfaces.IDatabaseObject;

        List<Position>? ReadPositionsByAssignmentPKs(List<int>? assignmentPKs);

        Vertriebsmitarbeiter? ReadUserByUserId(string userId);

        void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : Interfaces.IDatabaseObject;

        #endregion Public Methods
    }
}