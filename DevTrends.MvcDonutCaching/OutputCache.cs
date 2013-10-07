using System;
using System.Configuration;
using System.Web.Caching;

namespace DevTrends.MvcDonutCaching
{
    public sealed class OutputCache
    {
        static OutputCache()
        {
            var providerSettings = new CacheSettingsManager().RetrieveOutputCacheProviderSettings();

            if (providerSettings == null || providerSettings.Type == null)
            {
                Instance = new MemoryCacheProvider();
            }
            else
            {
                try
                {
                    Instance = (OutputCacheProvider)Activator.CreateInstance(Type.GetType(providerSettings.Type));
                    Instance.Initialize(providerSettings.Name, providerSettings.Parameters);

                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("Unable to instantiate and initialize OutputCacheProvider of type '{0}'. Make sure you are specifying the full type name.", providerSettings.Type),
                        ex
                    );
                }
            }
        }

        private OutputCache()
        {
        }

        public static OutputCacheProvider Instance
        {
            get;
            private set;
        }
    }
}
