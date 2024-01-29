using System.Security.Claims;
using VTAuftragserfassung.Database.DataAccess.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess.Services.Interfaces
{
    public interface ISessionService
    {
        #region Public Methods

        List<T>? GetSessionModels<T>(T? model, string sKey) where T : IDatabaseObject;

        ClaimsPrincipal? GetSessionUser();

        List<T>? SetSessionModels<T>(string sKey, List<T>? newModelData) where T : IDatabaseObject;

        #endregion Public Methods
    }
}