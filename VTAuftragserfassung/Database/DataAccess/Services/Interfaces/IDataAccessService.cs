using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess.Services.Interfaces
{
    public interface IDataAccessService
    {
        #region Public Methods

        int CreateSingle<T>(T? dbModel) where T : IDatabaseObject;

        void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject;

        List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject;

        int ReadScalar(string cmd);

        T? ReadSingle<T>(T? dbModel, string cmd) where T : IDatabaseObject;

        void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject;

        #endregion Public Methods
    }
}