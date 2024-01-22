using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess.Services.Interfaces
{
    public interface IDataAccessService
    {
        int ReadScalar(string cmd);
        #region Public Methods

        void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject;

        int CreateSingle<T>(T? dbModel) where T : IDatabaseObject;

        List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject;

        T? ReadSingle<T>(T? dbModel, string cmd) where T : IDatabaseObject;

        void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject;

        #endregion Public Methods
    }
}