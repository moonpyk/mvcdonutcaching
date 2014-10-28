using System;
using System.Configuration;
using System.Diagnostics;
using System.Security;
using System.Web;
using System.Web.Configuration;

namespace DevTrends.MvcDonutCaching
{
    public class CacheSettingsManager : ICacheSettingsManager
    {
        private const string AspnetInternalProviderName = "AspNetInternalProvider";
        private readonly OutputCacheSection _outputCacheSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSettingsManager"/> class.
        /// </summary>
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

        /// <summary>
        /// Returns the output cache provider settings.
        /// </summary>
        /// <returns>
        /// A <see cref="ProviderSettings" /> instance.
        /// </returns>
        public ProviderSettings RetrieveOutputCacheProviderSettings()
        {
            return _outputCacheSection.DefaultProviderName == AspnetInternalProviderName 
                ? null 
                : _outputCacheSection.Providers[_outputCacheSection.DefaultProviderName];
        }

        /// <summary>
        /// Returns an output cache profile for the asked <see cref="cacheProfileName" />.
        /// </summary>
        /// <param name="cacheProfileName">Name of the cache profile.</param>
        /// <returns>
        /// A <see cref="OutputCacheProfile" /> instance.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">MvcDonutCaching does not have permission to read web.config section 'OutputCacheSettingsSection'.</exception>
        /// <exception cref="System.Web.HttpException"></exception>
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

        /// <summary>
        /// Return a value indicating whether caching is globally enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if caching is globally enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCachingEnabledGlobally
        {
            get { return _outputCacheSection.EnableOutputCache; }
        }
    }
}
