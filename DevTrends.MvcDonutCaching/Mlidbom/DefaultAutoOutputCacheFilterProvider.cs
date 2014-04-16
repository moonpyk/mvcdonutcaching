namespace DevTrends.MvcDonutCaching.Mlidbom
{
    /// <summary>
    /// Simple implementation of AutoOutputCacheFilterProvider that simply manually creates the filter with default dependencies and assigns the supplied attribute as the Configuration property.
    /// </summary>
    public class DefaultAutoOutputCacheFilterProvider : AutoOutputCacheFilterProvider
    {
        protected override AutoOutputCacheFilter CreateFilterInstance(AutoOutputCacheAttribute attribute)
        {
            return new AutoOutputCacheFilter(
                new KeyGenerator(new KeyBuilder()),
                new OutputCacheManager(OutputCache.Instance, new KeyBuilder()),
                new CacheSettingsManager())
                   {
                       Config = attribute
                   };
        }
    }
}
