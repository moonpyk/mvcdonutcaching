using System;
using System.Configuration;
using System.Web.Configuration;

namespace DevTrends.MvcDonutCaching
{
    public interface ICacheSettingsManager
    {
        [Obsolete("Not used in the library anymore, in favor of RetrieveOutputCacheProviderSettings(), will be removed in 1.4.x branch")]
        string RetrieveOutputCacheProviderType();
        ProviderSettings RetrieveOutputCacheProviderSettings();
        OutputCacheProfile RetrieveOutputCacheProfile(string cacheProfileName);

        bool IsCachingEnabledGlobally { get; }
    }
}
