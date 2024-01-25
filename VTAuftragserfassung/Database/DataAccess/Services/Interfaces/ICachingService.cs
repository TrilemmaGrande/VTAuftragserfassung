using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess.Services.Interfaces
{
    public interface ICachingService
    {
        #region Public Methods

        List<T>? GetCachedModels<T>(T? model) where T : IDatabaseObject;

        bool InvalidateCacheModels<T>(T? model, string cKey = "");

        List<T>? UpdateCachedModels<T>(List<T>? newModelData) where T : IDatabaseObject;

        #endregion Public Methods
    }
}