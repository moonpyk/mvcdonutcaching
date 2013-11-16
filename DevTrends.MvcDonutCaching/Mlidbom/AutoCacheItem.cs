namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class AutoCacheItem : CacheItem
    {
        //Todo: move or get along with serialization etc....
        public IDonut Donut { get; set; }
    }
}