using System;

namespace DevTrends.MvcDonutCaching
{
    internal interface IExtendedOutputCacheManager : IOutputCacheManager
    {
        void AddItem(string key, ICacheItem cacheItem, DateTime utcExpiry);
        ICacheItem GetItem(string key);
    }
}
