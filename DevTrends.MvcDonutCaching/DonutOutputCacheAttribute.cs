using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace DevTrends.MvcDonutCaching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DonutOutputCacheAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private readonly ICacheHeadersHelper _cacheHeadersHelper;
        private readonly ICacheSettingsManager _cacheSettingsManager;
        private readonly IDonutHoleFiller _donutHoleFiller;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IExtendedOutputCacheManager _outputCacheManager;
        private CacheSettings _cacheSettings;
        private bool? _noStore;
        private OutputCacheOptions? _options;

        public DonutOutputCacheAttribute() : this(new KeyBuilder()) { }

        public DonutOutputCacheAttribute(IKeyBuilder keyBuilder)
        {
            _keyGenerator         = new KeyGenerator(keyBuilder);
            _outputCacheManager   = new OutputCacheManager(OutputCache.Instance, keyBuilder);
            _donutHoleFiller      = new DonutHoleFiller(new EncryptingActionSettingsSerialiser(new ActionSettingsSerialiser(), new Encryptor()));
            _cacheSettingsManager = new CacheSettingsManager();
            _cacheHeadersHelper   = new CacheHeadersHelper();

            Duration = -1;
            Location = (OutputCacheLocation)(-1);
            Options = OutputCache.DefaultOptions;
        }

        /// <summary>
        /// Gets or sets the cache duration, in seconds.
        /// </summary>
        public int Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vary-by-param value.
        /// </summary>
        public string VaryByParam
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vary-by-custom value.
        /// </summary>
        public string VaryByCustom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cache profile name.
        /// </summary>
        public string CacheProfile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public OutputCacheLocation Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to store the cache.
        /// </summary>
        public bool NoStore
        {
            get
            {
                return _noStore ?? false;
            }
            set
            {
                _noStore = value;
            }
        }

        /// <summary>
        /// Get or sets the <see cref="OutputCacheOptions"/> for this attributes. Specifying a value here will
        /// make the <see cref="OutputCache.DefaultOptions"/> value ignored.
        /// </summary>
        public OutputCacheOptions Options
        {
            get
            {
                return _options ?? OutputCacheOptions.None;
            }
            set
            {
                _options = value;
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (_cacheSettings != null)
            {
                ExecuteCallback(filterContext, true);
            }
        }

        /// <summary>
        /// Called before an action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _cacheSettings = BuildCacheSettings();

            var cacheKey = _keyGenerator.GenerateKey(filterContext, _cacheSettings);

            // Are we actually storing data on the server side ?
            if (_cacheSettings.IsServerCachingEnabled)
            {
                CacheItem cachedItem = null;

                // If the request is a POST, we lookup for NoCacheLookupForPosts option
                // We are fetching the stored value only if the option has not been set and the request is not a POST
                if (
                    (_cacheSettings.Options & OutputCacheOptions.NoCacheLookupForPosts) != OutputCacheOptions.NoCacheLookupForPosts ||
                    filterContext.HttpContext.Request.HttpMethod != "POST"
                )
                {
                    cachedItem = _outputCacheManager.GetItem(cacheKey);
                }

                // We have a cached version on the server side
                if (cachedItem != null)
                {
                    // We inject the previous result into the MVC pipeline
                    // The MVC action won't execute as we injected the previous cached result.
                    filterContext.Result = new ContentResult
                    {
                        Content = _donutHoleFiller.ReplaceDonutHoleContent(cachedItem.Content, filterContext, _cacheSettings.Options),
                        ContentType = cachedItem.ContentType
                    };
                }
            }

            // Did we already injected something ?
            if (filterContext.Result != null)
            {
                return; // No need to continue 
            }

            // We are hooking into the pipeline to replace the response Output writer
            // by something we own and later eventually gonna cache
            var cachingWriter = new StringWriter(CultureInfo.InvariantCulture);

            var originalWriter = filterContext.HttpContext.Response.Output;

            filterContext.HttpContext.Response.Output = cachingWriter;

            // Will be called back by OnResultExecuted -> ExecuteCallback
            filterContext.HttpContext.Items[cacheKey] = new Action<bool>(hasErrors =>
            {
                // Removing this executing action from the context
                filterContext.HttpContext.Items.Remove(cacheKey);

                // We restore the original writer for response
                filterContext.HttpContext.Response.Output = originalWriter;

                if (hasErrors)
                {
                    return; // Something went wrong, we are not going to cache something bad
                }

                // Now we use owned caching writer to actually store data
                var cacheItem = new CacheItem
                {
                    Content = cachingWriter.ToString(),
                    ContentType = filterContext.HttpContext.Response.ContentType
                };

                filterContext.HttpContext.Response.Write(
                    _donutHoleFiller.RemoveDonutHoleWrappers(cacheItem.Content, filterContext, _cacheSettings.Options)
                );

                if (_cacheSettings.IsServerCachingEnabled && filterContext.HttpContext.Response.StatusCode == 200)
                {
                    _outputCacheManager.AddItem(cacheKey, cacheItem, DateTime.UtcNow.AddSeconds(_cacheSettings.Duration));
                }
            });
        }

        /// <summary>
        /// Called after an action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (_cacheSettings == null)
            {
                return;
            }

            // See OnActionExecuting
            ExecuteCallback(filterContext, filterContext.Exception != null);

            // If we are in the context of a child action, the main action is responsible for setting
            // the right HTTP Cache headers for the final response.
            if (!filterContext.IsChildAction)
            {
                _cacheHeadersHelper.SetCacheHeaders(filterContext.HttpContext.Response, _cacheSettings);
            }
        }

        protected CacheSettings BuildCacheSettings()
        {
            CacheSettings cacheSettings;

            if (string.IsNullOrEmpty(CacheProfile))
            {
                cacheSettings = new CacheSettings
                {
                    IsCachingEnabled = _cacheSettingsManager.IsCachingEnabledGlobally,
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
                var cacheProfile = _cacheSettingsManager.RetrieveOutputCacheProfile(CacheProfile);

                cacheSettings = new CacheSettings
                {
                    IsCachingEnabled = _cacheSettingsManager.IsCachingEnabledGlobally && cacheProfile.Enabled,
                    Duration = Duration == -1 ? cacheProfile.Duration : Duration,
                    VaryByCustom = VaryByCustom ?? cacheProfile.VaryByCustom,
                    VaryByParam = VaryByParam ?? cacheProfile.VaryByParam,
                    Location = (int)Location == -1 ? ((int)cacheProfile.Location == -1 ? OutputCacheLocation.Server : cacheProfile.Location) : Location,
                    NoStore = _noStore.HasValue ? _noStore.Value : cacheProfile.NoStore,
                    Options = Options,
                };
            }

            if (cacheSettings.Duration == -1)
            {
                throw new HttpException("The directive or the configuration settings profile must specify the 'duration' attribute.");
            }

            if (cacheSettings.Duration < 0)
            {
                throw new HttpException("The 'duration' attribute must have a value that is greater than or equal to zero.");
            }

            return cacheSettings;
        }

        private void ExecuteCallback(ControllerContext context, bool hasErrors)
        {
            var cacheKey = _keyGenerator.GenerateKey(context, _cacheSettings);

            var callback = context.HttpContext.Items[cacheKey] as Action<bool>;

            if (callback != null)
            {
                callback.Invoke(hasErrors);
            }
        }
    }
}
