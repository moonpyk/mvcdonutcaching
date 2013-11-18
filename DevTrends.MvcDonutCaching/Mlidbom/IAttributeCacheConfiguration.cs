using System.Web.UI;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    /// <summary>
    /// Implemented by attributes that let a developer specify cache settings for a controller or controller action
    /// </summary>
    public interface IAttributeCacheConfiguration
    {
        /// <summary>Gets or sets the cache duration, in seconds.</summary>
        double Duration { get; }

        /// <summary>Gets or sets the vary-by-param value.</summary>
        string VaryByParam { get; }

        /// <summary>Gets or sets the vary-by-custom value.</summary>
        string VaryByCustom { get; }

        /// <summary>Gets or sets the cache profile name.</summary>
        string CacheProfile { get; }

        /// <summary>Gets or sets the location.</summary>
        OutputCacheLocation Location { get; }

        /// <summary>Gets or sets a value that indicates whether to store the cache.</summary>
        bool? NoStore { get; }

        /// <summary>
        /// Get or sets the <see cref="OutputCacheOptions"/> for this attributes. Specifying a value here will
        /// make the <see cref="OutputCache.DefaultOptions"/> value ignored.
        /// </summary>
        OutputCacheOptions Options { get; set; }
    }
}