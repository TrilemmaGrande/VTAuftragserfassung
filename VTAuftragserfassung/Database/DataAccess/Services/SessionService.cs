using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISession? _session;
        public SessionService(IHttpContextAccessor sessionAccess)
        {
            _session = sessionAccess?.HttpContext?.Session;
        }
        public List<T>? GetSessionModels<T>(T? model, string sKey) where T : IDatabaseObject
        {
            if (!string.IsNullOrEmpty(sKey) && _session != null
                && _session.TryGetValue(sKey, out byte[]? cachedModelJson)
                && cachedModelJson.Length > 0)
            {
                return CompressorExtension.DecompressAndDeserialize<List<T>>(cachedModelJson);
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
                _session.Set(sKey, CompressorExtension.SerializeAndCompress(newModelData));
            }
            return newModelData;
        }
    }
}
