using System.Configuration;
using System.Web.Configuration;

namespace DevTrends.MvcDonutCaching
{
    public interface ICacheSettingsManager
    {
        ProviderSettings RetrieveOutputCacheProviderSettings();
        OutputCacheProfile RetrieveOutputCacheProfile(string cacheProfileName);

        bool IsCachingEnabledGlobally { get; }
    }
}
