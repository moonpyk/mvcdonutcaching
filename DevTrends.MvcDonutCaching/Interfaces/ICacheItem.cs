namespace DevTrends.MvcDonutCaching
{
    public interface ICacheItem
    {
        /// <summary>
        /// Gets or sets content type.
        /// </summary>
        /// <value>
        /// The content type.
        /// </value>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content to be cached.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        string Content { get; set; }
    }
}
