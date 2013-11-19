using System;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AutoOutputCacheAttribute : ActionFilterAttribute, IExceptionFilter, IAttributeCacheConfiguration
    {
        // Protected
        protected readonly ICacheSettingsManager CacheSettingsManager;
        protected readonly IKeyGenerator KeyGenerator;
        protected readonly IReadWriteOutputCacheManager OutputCacheManager;

        protected CacheSettings EffectiveCacheSettings
        {
            get
            {
                if(_cacheSettings == null)
                {
                     //This cannot be done in the constructor because the properties of the attribute are not yet set to the values specified by the programmer.
                    _cacheSettings = CacheSettingsManager.BuildEffectiveSettingsCombinedWithGlobalConfiguration(this);
                }
                return _cacheSettings;
            }
        }

        // Private
        private OutputCacheOptions? _options;
        private CacheSettings _cacheSettings;

        public AutoOutputCacheAttribute() : this(new KeyBuilder()) {}

        public AutoOutputCacheAttribute(IKeyBuilder keyBuilder) :
            this(new KeyGenerator(keyBuilder),
            new OutputCacheManager(OutputCache.Instance, keyBuilder),
            new CacheSettingsManager()
            ) {}

        protected AutoOutputCacheAttribute(
            IKeyGenerator keyGenerator,
            IReadWriteOutputCacheManager outputCacheManager,
            ICacheSettingsManager cacheSettingsManager
            )
        {            
            KeyGenerator = keyGenerator;
            OutputCacheManager = outputCacheManager;
            CacheSettingsManager = cacheSettingsManager;

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
        public OutputCacheOptions Options { get { return _options ?? OutputCacheOptions.None; } set { _options = value; } }        
        
        override public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cacheKey = KeyGenerator.GenerateKey(filterContext, EffectiveCacheSettings);

            // Are we actually storing data on the server side ?
            if(EffectiveCacheSettings.IsServerCachingEnabled)
            {
                // If the request is a POST, we lookup for NoCacheLookupForPosts option
                // We are fetching the stored value only if the option has not been set and the request is not a POST
                if(!EffectiveCacheSettings.Options.HasFlag(OutputCacheOptions.NoCacheLookupForPosts) || filterContext.HttpContext.Request.HttpMethod != "POST")
                {
                    var cachedItem = (AutoCacheItem)OutputCacheManager.GetItem(cacheKey);
                    if (cachedItem != null)// We have a cached version on the server side
                    {
                        var executor = DonutOutputManager.ActionExecutingCached(filterContext, cachedItem.Donut);
                        // We inject the previous result into the MVC pipeline
                        // The MVC action won't execute as we injected the previous cached result.

                        filterContext.Result = new ContentResult
                                               {
                                                   Content = executor.Execute(filterContext),
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
            if (filterContext.Exception != null)//We handle exceptions in OnException trying to handle them more than once will end badly.
            {
                return;
            }

            var donut = DonutOutputManager.ResultExecutionSucceeded(filterContext);

            if (donut.Cached //Item is already cached and we do not want to extend its lifetime by inserting it with a new expiration
                || !EffectiveCacheSettings.IsServerCachingEnabled //Caching is disabled
                || filterContext.HttpContext.Response.StatusCode != (int)HttpStatusCode.OK)//Page is not returning content.
            {
                return; 
            }

            OutputCacheManager.AddItem(
                key: KeyGenerator.GenerateKey(filterContext, EffectiveCacheSettings), 
                cacheItem: new AutoCacheItem(donut, filterContext.HttpContext.Response.ContentType),
                utcExpiry: DateTime.UtcNow.AddSeconds(EffectiveCacheSettings.Duration));
        }

        public void OnException(ExceptionContext filterContext)
        {
            DonutOutputManager.ResultExecutionFailed(filterContext);
        }
    }
}
