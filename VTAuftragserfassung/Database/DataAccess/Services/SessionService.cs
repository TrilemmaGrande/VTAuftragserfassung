using System.Security.Claims;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    /// <summary>
    /// [Relationship]: connects Webserver-Session-Cache with application.
    /// [Input]: DataBaseObjects.
    /// [Output]: DataBaseObjects, SessionUser.
    /// [Dependencies]: uses ISession, HttpContextAceessor and Compression-Extension.
    /// [Notice]: - 
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly ISession? _session;
        private readonly ClaimsPrincipal? _sessionUser;

        public SessionService(IHttpContextAccessor sessionAccess)
        {
            _session = sessionAccess?.HttpContext?.Session;
            _sessionUser = sessionAccess?.HttpContext?.User;
        }

        public ClaimsPrincipal? GetSessionUser() => _sessionUser;

        public List<T>? GetSessionModels<T>(T? model, string sKey) 
        {
            if (!string.IsNullOrEmpty(sKey) && _session != null
                && _session.TryGetValue(sKey, out byte[]? cachedModelJson)
                && cachedModelJson.Length > 0)
            {
                return cachedModelJson.DecompressAndDeserialize<List<T>>();
            }
            return null;
        }

        public List<T>? SetSessionModels<T>(string sKey, List<T>? newModelData) 
        {
            if (string.IsNullOrEmpty(sKey) || _session == null)
            {
                return null;
            }
            if (newModelData != null)
            {
                _session.Set(sKey, newModelData.SerializeAndCompress());
            }
            return newModelData;
        }
    }
}