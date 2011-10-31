using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevTrends.MvcDonutCaching
{
    public interface IOutputCacheManager
    {
        void AddItem(string key, object entry, DateTime utcExpiry);
        string GetItem(string key);
    }
}
