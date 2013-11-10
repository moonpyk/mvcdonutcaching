using System;

namespace DevTrends.MvcDonutCaching
{
    public interface IReadWriteOutputCacheManager : IOutputCacheManager
    {
        void AddItem(string key, ICacheItem cacheItem, DateTime utcExpiry);
        ICacheItem GetItem(string key);
    }
}
