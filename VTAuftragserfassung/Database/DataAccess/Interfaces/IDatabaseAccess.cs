using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess.Services.Interfaces
{
    public interface IDatabaseAccess
    {
        #region Public Methods

        void CreateAll<T>(List<KeyValuePair<T, string>> dbModelsWithCmds) where T : IDatabaseObject;

        int CreateSingle<T>(T? dbModel, string cmd) where T : IDatabaseObject;

        List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject;

        int ReadScalar(string cmd);

        T? ReadSingle<T>(T? dbModel, string cmd) where T : IDatabaseObject;

        void Update<T>(T? dbModel, string cmd) where T : IDatabaseObject;

        #endregion Public Methods
    }
}