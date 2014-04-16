using System;
using System.Configuration;
using System.Diagnostics;
using System.Security;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace DevTrends.MvcDonutCaching
{
    public class CacheSettingsManager : ICacheSettingsManager
    {
        private const string AspnetInternalProviderName = "AspNetInternalProvider";
        private readonly OutputCacheSection _outputCacheSection;

        public CacheSettingsManager()
        {
            try
            {
                _outputCacheSection = (OutputCacheSection)ConfigurationManager.GetSection("system.web/caching/outputCache");
            }
            catch (SecurityException)
            {
                Trace.WriteLine("MvcDonutCaching does not have permission to read web.config section 'OutputCacheSection'. Using default provider.");
                _outputCacheSection = new OutputCacheSection
                {
                    DefaultProviderName = AspnetInternalProviderName,
                    EnableOutputCache = true
                };
            }
        }

        public ProviderSettings RetrieveOutputCacheProviderSettings()
        {
            return _outputCacheSection.DefaultProviderName == AspnetInternalProviderName 
                ? null 
                : _outputCacheSection.Providers[_outputCacheSection.DefaultProviderName];
        }

        public OutputCacheProfile RetrieveOutputCacheProfile(string cacheProfileName)
        {
            OutputCacheSettingsSection outputCacheSettingsSection;

            try
            {
                outputCacheSettingsSection = (OutputCacheSettingsSection)ConfigurationManager.GetSection("system.web/caching/outputCacheSettings");
            }
            catch (SecurityException)
            {
                throw new SecurityException("MvcDonutCaching does not have permission to read web.config section 'OutputCacheSettingsSection'.");
            }

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

        //todo: Use this from DonutOutputCacheAttribute rather than re-implement it there.
        public CacheSettings BuildEffectiveSettingsCombinedWithGlobalConfiguration(IAttributeCacheConfiguration attribute)
        {
            CacheSettings cacheSettings;

            if (string.IsNullOrEmpty(attribute.CacheProfile))
            {
                cacheSettings = new CacheSettings
                {
                    IsCachingEnabled = IsCachingEnabledGlobally,
                    Duration = attribute.Duration,
                    VaryByCustom = attribute.VaryByCustom,
                    VaryByParam = attribute.VaryByParam,
                    Location = (int)attribute.Location == -1 ? OutputCacheLocation.Server : attribute.Location,
                    NoStore = attribute.NoStore ?? false,
                    Options = attribute.Options,
                };
            }
            else
            {
                var cacheProfile = RetrieveOutputCacheProfile(attribute.CacheProfile);

                cacheSettings = new CacheSettings
                {
                    IsCachingEnabled = IsCachingEnabledGlobally && cacheProfile.Enabled,
                    Duration = (int)attribute.Duration == -1 ? cacheProfile.Duration : attribute.Duration,
                    VaryByCustom = attribute.VaryByCustom ?? cacheProfile.VaryByCustom,
                    VaryByParam = attribute.VaryByParam ?? cacheProfile.VaryByParam,
                    Location = (int)attribute.Location == -1 ? ((int)cacheProfile.Location == -1 ? OutputCacheLocation.Server : cacheProfile.Location) : attribute.Location,
                    NoStore = attribute.NoStore ?? cacheProfile.NoStore,
                    Options = attribute.Options,
                };
            }

            if ((int)cacheSettings.Duration == -1)
            {
                throw new HttpException("The directive or the configuration settings profile must specify the 'duration' attribute.");
            }

            if (cacheSettings.Duration < 0)
            {
                throw new HttpException("The 'duration' attribute must have a value that is greater than or equal to zero.");
            }

            return cacheSettings;
        }
    }
}
