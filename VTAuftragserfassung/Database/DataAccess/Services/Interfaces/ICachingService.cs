using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess.Services.Interfaces
{
    public interface ICachingService
    {
        #region Public Methods

        List<T>? GetCachedModels<T>(T? model);

        bool InvalidateCacheModels<T>(T? model, string cKey = "");

        List<T>? UpdateCachedModels<T>(List<T>? newModelData);

        #endregion Public Methods
    }
}