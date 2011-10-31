
namespace DevTrends.MvcDonutCaching
{
    public class CacheSettings
    {
        public bool IsCachingEnabled { get; set; }
        public int Duration { get; set; }
        public string VaryByParam { get; set; }
        public string VaryByCustom { get; set; }
    }
}
