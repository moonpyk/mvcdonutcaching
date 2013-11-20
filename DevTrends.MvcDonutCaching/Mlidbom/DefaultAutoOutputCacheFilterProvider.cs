namespace DevTrends.MvcDonutCaching.Mlidbom
{
    /// <summary>
    /// Simple implementation of AutoOutputCacheFilterProvider that simply manually creates the filter with default dependencies.
    /// </summary>
    public class DefaultAutoOutputCacheFilterProvider : AutoOutputCacheFilterProvider
    {
        protected override AutoOutputCacheFilter CreateFilterInstance(AutoOutputCacheAttribute autoOutputCacheAttribute)
        {
            return new AutoOutputCacheFilter(
                autoOutputCacheAttribute,
                new KeyGenerator(new KeyBuilder()),
                new OutputCacheManager(OutputCache.Instance, new KeyBuilder()),
                new CacheSettingsManager());
        }
    }
}
