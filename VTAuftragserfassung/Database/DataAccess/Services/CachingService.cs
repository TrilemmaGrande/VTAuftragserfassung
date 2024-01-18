using Microsoft.Extensions.Caching.Memory;
using System.Resources;
using System.Transactions;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class CachingService : ICachingService
    {
        #region Private Fields

        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        #endregion Private Fields

        #region Public Constructors

        public CachingService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
        }

        #endregion Public Constructors

        #region Public Methods

        private string GenerateCacheKey<T>(T? model)
        {
            return model != null ? model.GetType().Name : string.Empty;
        }
        public List<T>? GetCachedModels<T>(T? model) where T : IDatabaseObject
        {
            string cKey = GenerateCacheKey(model);
            if (!string.IsNullOrEmpty(cKey)
                && _memoryCache.TryGetValue(cKey, out byte[]? cachedModelJson)
                && cachedModelJson != null)
            {
                return CompressorExtension.DecompressAndDeserialize<List<T>>(cachedModelJson);
            }
            return null;
        }

        public bool InvalidateCacheModels<T>(T? model, string cKey = "")
        {
            cKey = string.IsNullOrEmpty(cKey) ? GenerateCacheKey(model) : cKey;
            if (!string.IsNullOrEmpty(cKey))
            {
                _memoryCache.Remove(cKey);
                return true;
            }
            return false;
        }

        public List<T>? UpdateCachedModels<T>(List<T>? newModelData) where T : IDatabaseObject
        {
            using (TransactionScope scope = new())
            {
                T? model = newModelData != null ? newModelData.FirstOrDefault() : default;
                string cKey = GenerateCacheKey(model);

                if (newModelData != null && !string.IsNullOrEmpty(cKey) && InvalidateCacheModels(model, cKey))
                {
                    _memoryCache.Set(cKey, CompressorExtension.SerializeAndCompress(newModelData), _cacheEntryOptions);
                    scope.Complete();
                    return newModelData;
                }
            }
            return null;
        }
        #endregion Public Methods
    }
}