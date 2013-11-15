using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AutoOutputCacheAttribute : ActionFilterAttribute
    {
        // Protected
        protected readonly ICacheHeadersHelper CacheHeadersHelper;
        protected readonly ICacheSettingsManager CacheSettingsManager;
        protected readonly IDonutHoleFiller DonutHoleFiller;
        protected readonly IKeyGenerator KeyGenerator;
        protected readonly IReadWriteOutputCacheManager OutputCacheManager;

        protected CacheSettings CacheSettings
        {
            get
            {
                if(_cacheSettings == null)
                {
                    _cacheSettings = BuildCacheSettings();
                }
                return _cacheSettings;
            }
        }

        // Private
        private bool? _noStore;
        private OutputCacheOptions? _options;
        private CacheSettings _cacheSettings;

        public AutoOutputCacheAttribute() : this(new KeyBuilder()) {}

        public AutoOutputCacheAttribute(IKeyBuilder keyBuilder) :
            this(
            new KeyGenerator(keyBuilder),
            new OutputCacheManager(OutputCache.Instance, keyBuilder),
            new DonutHoleFiller(new EncryptingActionSettingsSerialiser(new ActionSettingsSerialiser(), new Encryptor())),
            new CacheSettingsManager(),
            new CacheHeadersHelper()
            ) {}

        protected AutoOutputCacheAttribute(
            IKeyGenerator keyGenerator,
            IReadWriteOutputCacheManager outputCacheManager,
            IDonutHoleFiller donutHoleFiller,
            ICacheSettingsManager cacheSettingsManager,
            ICacheHeadersHelper cacheHeadersHelper
            )
        {
            KeyGenerator = keyGenerator;
            OutputCacheManager = outputCacheManager;
            DonutHoleFiller = donutHoleFiller;
            CacheSettingsManager = cacheSettingsManager;
            CacheHeadersHelper = cacheHeadersHelper;

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
        public bool NoStore { get { return _noStore ?? false; } set { _noStore = value; } }

        /// <summary>
        /// Get or sets the <see cref="OutputCacheOptions"/> for this attributes. Specifying a value here will
        /// make the <see cref="OutputCache.DefaultOptions"/> value ignored.
        /// </summary>
        public OutputCacheOptions Options { get { return _options ?? OutputCacheOptions.None; } set { _options = value; } }

        public bool ExcludeFromParentCache { get; set; }

        /// <summary>
        /// Builds the cache settings.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Web.HttpException">
        /// The 'duration' attribute must have a value that is greater than or equal to zero.
        /// </exception>
        protected CacheSettings BuildCacheSettings()
        {
            CacheSettings cacheSettings;

            if(string.IsNullOrEmpty(CacheProfile))
            {
                cacheSettings = new CacheSettings
                                {
                                    IsCachingEnabled = CacheSettingsManager.IsCachingEnabledGlobally,
                                    Duration = Duration,
                                    VaryByCustom = VaryByCustom,
                                    VaryByParam = VaryByParam,
                                    Location = (int)Location == -1 ? OutputCacheLocation.Server : Location,
                                    NoStore = NoStore,
                                    Options = Options,
                                };
            }
            else
            {
                var cacheProfile = CacheSettingsManager.RetrieveOutputCacheProfile(CacheProfile);

                cacheSettings = new CacheSettings
                                {
                                    IsCachingEnabled = CacheSettingsManager.IsCachingEnabledGlobally && cacheProfile.Enabled,
                                    Duration = Duration == -1 ? cacheProfile.Duration : Duration,
                                    VaryByCustom = VaryByCustom ?? cacheProfile.VaryByCustom,
                                    VaryByParam = VaryByParam ?? cacheProfile.VaryByParam,
                                    Location = (int)Location == -1 ? ((int)cacheProfile.Location == -1 ? OutputCacheLocation.Server : cacheProfile.Location) : Location,
                                    NoStore = _noStore.HasValue ? _noStore.Value : cacheProfile.NoStore,
                                    Options = Options,
                                };
            }

            if(cacheSettings.Duration == -1)
            {
                throw new HttpException("The directive or the configuration settings profile must specify the 'duration' attribute.");
            }

            if(cacheSettings.Duration < 0)
            {
                throw new HttpException("The 'duration' attribute must have a value that is greater than or equal to zero.");
            }

            return cacheSettings;
        }


        override public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cacheKey = KeyGenerator.GenerateKey(filterContext, CacheSettings);

            // Are we actually storing data on the server side ?
            if (CacheSettings.IsServerCachingEnabled)
            {
                // If the request is a POST, we lookup for NoCacheLookupForPosts option
                // We are fetching the stored value only if the option has not been set and the request is not a POST
                if (!CacheSettings.Options.HasFlag(OutputCacheOptions.NoCacheLookupForPosts) || filterContext.HttpContext.Request.HttpMethod != "POST")
                {
                    var cachedItem = OutputCacheManager.GetItem(cacheKey);
                    // We have a cached version on the server side
                    if (cachedItem != null)
                    {
                        // We inject the previous result into the MVC pipeline
                        // The MVC action won't execute as we injected the previous cached result.
                        filterContext.Result = new ContentResult
                        {
                            Content = cachedItem.Donut.Execute(filterContext),
                            ContentType = cachedItem.ContentType
                        };
                        return;
                    }
                }
            }

            DonutOutputManager.ActionExecuting(filterContext);
        }

        override public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var cacheKey = KeyGenerator.GenerateKey(filterContext, CacheSettings);
            var donut = DonutOutputManager.ResultExecuted(filterContext);
            var cacheItem = new CacheItem
                       {
                           Donut = donut,
                           ContentType = filterContext.HttpContext.Response.ContentType
                       };
            
            if (CacheSettings.IsServerCachingEnabled && filterContext.HttpContext.Response.StatusCode == 200)
            {
                OutputCacheManager.AddItem(cacheKey, cacheItem, DateTime.UtcNow.AddSeconds(CacheSettings.Duration));
            }
        }
    }
}
