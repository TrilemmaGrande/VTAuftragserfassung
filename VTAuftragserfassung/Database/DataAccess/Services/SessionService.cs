using Microsoft.AspNetCore.DataProtection.KeyManagement;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISession _session;
        public SessionService(IHttpContextAccessor sessionAccess)
        {
            _session = sessionAccess.HttpContext.Session;
        }
        public List<T>? GetSessionModels<T>(T? model, string sKey) where T : IDatabaseObject
        {
            if (!string.IsNullOrEmpty(sKey)
                && _session.TryGetValue(sKey, out byte[]? cachedModelJson)
                && cachedModelJson != null)
            {
                return CompressorExtension.DecompressAndDeserialize<List<T>>(cachedModelJson);
            }
            return null;
        }
        public List<T>? SetSessionModels<T>(string sKey, List<T>? newModelData) where T : IDatabaseObject
        {
            T? model = newModelData != null ? newModelData.FirstOrDefault() : default;
            if (string.IsNullOrEmpty(sKey))
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
