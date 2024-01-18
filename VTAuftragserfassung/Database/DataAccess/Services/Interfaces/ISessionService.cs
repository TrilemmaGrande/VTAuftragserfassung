using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess.Services.Interfaces
{
    public interface ISessionService
    {
        List<T>? GetSessionModels<T>(T? model, string sKey) where T : IDatabaseObject;
        List<T>? SetSessionModels<T>(string sKey, List<T>? newModelData) where T : IDatabaseObject;
    }
}
