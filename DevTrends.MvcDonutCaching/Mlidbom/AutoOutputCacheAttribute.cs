using System;
using System.Web.Mvc;
using System.Web.UI;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AutoOutputCacheAttribute : FilterAttribute, IAttributeCacheConfiguration
    {
        public AutoOutputCacheAttribute()
        {
            Duration = -1;
            Location = (OutputCacheLocation)(-1);
            Options = OutputCache.DefaultOptions;
        }

        /// <summary>Gets or sets the cache duration, in seconds.</summary>
        public double Duration { get; set; }

        /// <summary>Gets or sets the vary-by-param value.</summary>
        public string VaryByParam { get; set; }

        /// <summary>Gets or sets the vary-by-custom value.</summary>
        public string VaryByCustom { get; set; }

        /// <summary>Gets or sets the cache profile name.</summary>
        public string CacheProfile { get; set; }

        /// <summary>Gets or sets the location.</summary>
        public OutputCacheLocation Location { get; set; }

        /// <summary>Gets or sets a value that indicates whether to store the cache.</summary>
        public bool? NoStore { get; set; }

        /// <summary>
        /// Get or sets the <see cref="OutputCacheOptions"/> for this attributes. Specifying a value here will
        /// make the <see cref="OutputCache.DefaultOptions"/> value ignored.
        /// </summary>
        public OutputCacheOptions Options { get; set; }
    }
}
