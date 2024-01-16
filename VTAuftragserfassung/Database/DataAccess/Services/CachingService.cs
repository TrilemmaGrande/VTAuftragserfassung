using Microsoft.Extensions.Caching.Memory;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class CachingService : ICachingService
    {
        #region Private Fields

        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        private readonly IMemoryCache _memoryCache;

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
        public List<T>? GetCachedModel<T>(T? model) where T : IDatabaseObject
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

        public List<T>? UpdateCachedModel<T>(List<T>? newModelData) where T : IDatabaseObject
        {
            T? model = newModelData != null ? newModelData.FirstOrDefault() : default;
            string cKey = GenerateCacheKey(model);
            if (string.IsNullOrEmpty(cKey))
            {
                return null;
            }
            _memoryCache.Remove(cKey);
            if (newModelData != null)
            {
                _memoryCache.Set(cKey, CompressorExtension.SerializeAndCompress(newModelData), _cacheEntryOptions);
            }
            return newModelData;
        }
        #endregion Public Methods
    }
}