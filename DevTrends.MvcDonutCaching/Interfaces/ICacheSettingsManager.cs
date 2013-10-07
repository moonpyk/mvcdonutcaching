using System.Configuration;
using System.Web.Configuration;

namespace DevTrends.MvcDonutCaching
{
    public interface ICacheSettingsManager
    {
        string RetrieveOutputCacheProviderType();
        ProviderSettings RetrieveOutputCacheProviderSettings();
        OutputCacheProfile RetrieveOutputCacheProfile(string cacheProfileName);

        bool IsCachingEnabledGlobally { get; }
    }
}
