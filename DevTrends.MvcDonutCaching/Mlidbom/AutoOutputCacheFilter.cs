using System;
using System.Net;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class AutoOutputCacheFilter : IActionFilter, IResultFilter, IExceptionFilter
    {
        public AutoOutputCacheFilter(IAttributeCacheConfiguration config)
            : this(
                config,
                new KeyGenerator(new KeyBuilder()),
                new OutputCacheManager(OutputCache.Instance, new KeyBuilder()),
                new CacheSettingsManager()) {}

        public AutoOutputCacheFilter(
            IAttributeCacheConfiguration config,
            IKeyGenerator keyGenerator,
            IReadWriteOutputCacheManager outputCacheManager,
            ICacheSettingsManager cacheSettingsManager
            )
        {
            KeyGenerator = keyGenerator;
            EffectiveCacheSettings = cacheSettingsManager.BuildEffectiveSettingsCombinedWithGlobalConfiguration(config);
            OutputCacheManager = outputCacheManager;
        }

        private IKeyGenerator KeyGenerator { get; set; }
        private IReadWriteOutputCacheManager OutputCacheManager { get; set; }
        private CacheSettings EffectiveCacheSettings { get; set; }


        public void OnActionExecuting(ActionExecutingContext filterContext)
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
                    if(cachedItem != null) // We have a cached version on the server side
                    {
                        // We inject the previous result into the MVC pipeline
                        // The MVC action won't execute as we injected the previous cached result.
                        filterContext.Result = DonutOutputManager.ExecuteCachedResult(cachedItem, filterContext);
                        return;
                    }
                }
            }

            DonutOutputManager.ActionExecuting(filterContext);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {}

        public void OnResultExecuting(ResultExecutingContext filterContext) {}

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if(filterContext.Exception != null) //We handle exceptions in OnException trying to handle them more than once will end badly.
            {
                return;
            }

            var donut = DonutOutputManager.ResultExecutionSucceeded(filterContext);

            if(donut.Cached //Item is already cached and we do not want to extend its lifetime by inserting it with a new expiration
               || !EffectiveCacheSettings.IsServerCachingEnabled //Caching is disabled
               || filterContext.HttpContext.Response.StatusCode != (int)HttpStatusCode.OK) //Page is not returning content.
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
