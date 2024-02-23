using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;

namespace VTAuftragserfassung.Database.DataAccess.Interfaces
{
    public interface IDatabaseService
    {
        #region Public Methods

        int CountAssignmentsByUserId(string userId);

        int CreateSingle<T>(T? dbModel) where T : IDatabaseObject;

        void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject;

        List<T>? ReadAll<T>(T? dbModel) where T : IDatabaseObject;

        List<Auftrag>? ReadAssignmentsPaginatedByUserId(string userId, Pagination? pagination);

        T1? ReadObjectByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject;

        List<Position>? ReadPositionsByAssignmentPKs(List<int>? assignmentPKs);

        Vertriebsmitarbeiter? ReadUserByUserId(string userId);

        void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject;
        bool CheckUserAssignmentMapping(string userId, int assignmentPk);

        #endregion Public Methods
    }
}