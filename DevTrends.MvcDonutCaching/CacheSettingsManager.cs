using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace DevTrends.MvcDonutCaching
{
    public class CacheSettingsManager : ICacheSettingsManager
    {
        private const string AspnetInternalProviderName = "AspNetInternalProvider";
        private readonly OutputCacheSection _outputCacheSection;

        public CacheSettingsManager()
        {
            _outputCacheSection = (OutputCacheSection)ConfigurationManager.GetSection("system.web/caching/outputCache");
        }

        public string RetrieveOutputCacheProviderType()
        {
            if (_outputCacheSection.DefaultProviderName == AspnetInternalProviderName)
            {
                return null;
            }

            return _outputCacheSection.Providers[_outputCacheSection.DefaultProviderName].Type;
        }

        public OutputCacheProfile RetrieveOutputCacheProfile(string cacheProfileName)
        {
            var outputCacheSettingsSection = (OutputCacheSettingsSection)ConfigurationManager.GetSection("system.web/caching/outputCacheSettings");

            if (outputCacheSettingsSection != null && outputCacheSettingsSection.OutputCacheProfiles.Count > 0)
            {
                var cacheProfile = outputCacheSettingsSection.OutputCacheProfiles[cacheProfileName];

                if (cacheProfile != null)
                {
                    return cacheProfile;
                }
            }

            throw new HttpException(string.Format("The '{0}' cache profile is not defined.  Please define it in the configuration file.", cacheProfileName));
        }

        public bool IsCachingEnabledGlobally
        {
            get { return _outputCacheSection.EnableOutputCache; }
        }
    }
}
