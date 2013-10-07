using System;
using System.Configuration;
using System.Web.Caching;

namespace DevTrends.MvcDonutCaching
{
    public sealed class OutputCache
    {
        private static readonly OutputCacheProvider instance;

        static OutputCache()
        {
            var providerSettings = new CacheSettingsManager().RetrieveOutputCacheProviderSettings();
            var providerType = providerSettings.Type;

            if (providerType == null)
            {
                instance = new MemoryCacheProvider();
            }
            else
            {
                try
                {
                    instance = (OutputCacheProvider)Activator.CreateInstance(Type.GetType(providerType));
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException(string.Format("Unable to instantiate OutputCacheProvider of type '{0}'. Make sure you are specifying the full type name.", providerType), ex);
                }
                instance.Initialize(providerSettings.Name, providerSettings.Parameters);
            }
        }

        private OutputCache()
        {
        }

        public static OutputCacheProvider Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
