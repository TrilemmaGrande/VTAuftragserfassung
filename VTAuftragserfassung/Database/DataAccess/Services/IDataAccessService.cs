
namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public interface IDataAccessService
    {
        int Create<T>(T? dbModel) where T : IDatabaseObject;
        void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject;
        List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject;
        T? ReadSingle<T>(T? dbModel, string cmd) where T : IDatabaseObject;
        void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject;
    }
}
