using System;

namespace DevTrends.MvcDonutCaching
{
    public interface IReadWriteOutputCacheManager : IOutputCacheManager
    {
        void AddItem(string key, CacheItem cacheItem, DateTime utcExpiry);
        CacheItem GetItem(string key);
    }
}
