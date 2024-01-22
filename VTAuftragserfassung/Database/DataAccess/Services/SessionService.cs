using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class SessionService(IHttpContextAccessor _sessionAccess) : ISessionService
    {
        private readonly ISession? _session = _sessionAccess?.HttpContext?.Session;

        public List<T>? GetSessionModels<T>(T? model, string sKey) where T : IDatabaseObject
        {
            if (!string.IsNullOrEmpty(sKey) && _session != null
                && _session.TryGetValue(sKey, out byte[]? cachedModelJson)
                && cachedModelJson.Length > 0)
            {
                return cachedModelJson.DecompressAndDeserialize<List<T>>();
            }
            return null;
        }
        public List<T>? SetSessionModels<T>(string sKey, List<T>? newModelData) where T : IDatabaseObject
        {
            if (string.IsNullOrEmpty(sKey) || _session == null)
            {
                return null;
            }
            _session.Remove(sKey);
            if (newModelData != null)
            {
                _session.Set(sKey, newModelData.SerializeAndCompress());
            }
            return newModelData;
        }
    }
}
