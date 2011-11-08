using System;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DonutOutputCacheAttribute : ActionFilterAttribute
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly IDonutHoleFiller _donutHoleFiller;
        private readonly IActionOutputBuilder _actionOutputBuilder;
        private readonly IExtendedOutputCacheManager _outputCacheManager;
        private readonly ICacheSettingsManager _cacheSettingsManager;

        private CacheSettings _cacheSettings;
        private string _cacheKey;

        public int Duration { get; set; }
        public string VaryByParam { get; set; }
        public string VaryByCustom { get; set; }
        public string CacheProfile { get; set; }

        public DonutOutputCacheAttribute()
        {
            var keyBuilder = new KeyBuilder();
            _keyGenerator = new KeyGenerator(keyBuilder);
            _donutHoleFiller = new DonutHoleFiller(new ActionSettingsSerialiser());
            _actionOutputBuilder = new ActionOutputBuilder();
            _outputCacheManager = new OutputCacheManager(OutputCache.Instance, keyBuilder);
            _cacheSettingsManager = new CacheSettingsManager();
        }

        internal DonutOutputCacheAttribute(IKeyGenerator keyGenerator, IDonutHoleFiller donutHoleFiller, IActionOutputBuilder actionOutputBuilder, 
                                           IExtendedOutputCacheManager outputCacheManager, ICacheSettingsManager cacheSettingsManager)
        {
            _keyGenerator = keyGenerator;
            _donutHoleFiller = donutHoleFiller;
            _actionOutputBuilder = actionOutputBuilder;
            _outputCacheManager = outputCacheManager;
            _cacheSettingsManager = cacheSettingsManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _cacheSettings = BuildCacheSettings();

            if (_cacheSettings.IsCachingEnabled)
            {
                _cacheKey = _keyGenerator.GenerateKey(filterContext, _cacheSettings);

                var content = _outputCacheManager.GetItem(_cacheKey);

                if (content != null)
                {
                    filterContext.Result = new ContentResult { Content = _donutHoleFiller.ReplaceDonutHoleContent(content, filterContext) };
                }
            }
        }        

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResultBase; // only cache views and partials

            if (viewResult != null)
            {
                var content = _actionOutputBuilder.GetActionOutput(viewResult, filterContext);

                if (_cacheSettings.IsCachingEnabled)
                {
                    _outputCacheManager.AddItem(_cacheKey, content, DateTime.Now.AddSeconds(_cacheSettings.Duration));
                }

                filterContext.Result = new ContentResult { Content = _donutHoleFiller.ReplaceDonutHoleContent(content, filterContext) };
            }            
        }
        
        private CacheSettings BuildCacheSettings()
        {
            if (string.IsNullOrEmpty(CacheProfile))
            {
                return new CacheSettings
                {
                    IsCachingEnabled = _cacheSettingsManager.IsCachingEnabledGlobally,
                    Duration = Duration,
                    VaryByCustom = VaryByCustom,
                    VaryByParam = VaryByParam
                };                
            }

            var cacheProfile = _cacheSettingsManager.RetrieveOutputCacheProfile(CacheProfile);

            return new CacheSettings
            {
                IsCachingEnabled = _cacheSettingsManager.IsCachingEnabledGlobally && cacheProfile.Enabled,
                Duration = Duration == 0 ? cacheProfile.Duration : Duration,
                VaryByCustom = VaryByCustom ?? cacheProfile.VaryByCustom,
                VaryByParam = VaryByParam ?? cacheProfile.VaryByParam
            };
        }    
    }
}