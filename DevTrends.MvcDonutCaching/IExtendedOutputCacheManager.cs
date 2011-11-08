using System;

namespace DevTrends.MvcDonutCaching
{
    internal interface IExtendedOutputCacheManager : IOutputCacheManager
    {
        void AddItem(string key, object entry, DateTime utcExpiry);
        string GetItem(string key);
    }
}
