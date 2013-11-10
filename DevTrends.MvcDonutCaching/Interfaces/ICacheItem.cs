namespace DevTrends.MvcDonutCaching
{
    public interface ICacheItem
    {
        string ContentType { get; set; }
        string Content { get; set; }
    }
}
