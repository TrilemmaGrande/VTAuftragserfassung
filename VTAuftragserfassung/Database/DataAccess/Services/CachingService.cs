﻿using Microsoft.Extensions.Caching.Memory;
using System.Transactions;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _memoryCache;

        public CachingService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #region Private Fields

        private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        #endregion Private Fields

        #region Public Methods

        public List<T>? GetCachedModels<T>(T? model) where T : IDatabaseObject
        {
            string cKey = GenerateCacheKey(model);
            if (string.IsNullOrEmpty(cKey)
                || !_memoryCache.TryGetValue(cKey, out byte[]? cachedModelJson)
                || cachedModelJson == null)
            {
                return null;
            }
            return cachedModelJson.DecompressAndDeserialize<List<T>>();
        }

        public bool InvalidateCacheModels<T>(T? model, string cKey = "")
        {
            cKey = string.IsNullOrEmpty(cKey) ? GenerateCacheKey(model) : cKey;
            if (string.IsNullOrEmpty(cKey))
            {
                return false;
            }
            _memoryCache.Remove(cKey);
            return true;
        }

        public List<T>? UpdateCachedModels<T>(List<T>? newModelData) where T : IDatabaseObject
        {
            using TransactionScope scope = new();
            T? model = newModelData != null ? newModelData.FirstOrDefault() : default;
            string cKey = GenerateCacheKey(model);

            if (newModelData != null && !string.IsNullOrEmpty(cKey) && InvalidateCacheModels(model, cKey))
            {
                _memoryCache.Set(cKey, newModelData.SerializeAndCompress(), _cacheEntryOptions);
                scope.Complete();
                return newModelData;
            }

            return null;
        }

        #endregion Public Methods

        #region Private Methods

        private string GenerateCacheKey<T>(T? model)
        {
            return model != null ? model.GetType().Name : string.Empty;
        }

        #endregion Private Methods
    }
}