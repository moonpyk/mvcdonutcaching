namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class AutoCacheItem : CacheItem
    {
        public Donut Donut { get; set; }
        public AutoCacheItem(IDonut donut, string contentType)
        {
            Donut = donut.CloneForCache();
            ContentType = contentType;            
        }
    }
}