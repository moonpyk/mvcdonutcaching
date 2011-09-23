using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;

namespace DevTrends.MvcDonutCaching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DonutOutputCacheAttribute : ActionFilterAttribute
    {
        private const string AspnetInternalProviderName = "AspNetInternalProvider";
        private const string CacheKeyPrefix = "_d0nutc@che.";

        private static readonly Regex DonutHoles = new Regex("<!--Donut#(.*)#-->", RegexOptions.Compiled);
        private static readonly bool IsCachingEnabled;
        private static readonly OutputCacheProvider Cache;

        private string _key;

        public int Duration { get; set; }
        public string VaryByParam { get; set; }
        public string CacheProfile { get; set; }

        static DonutOutputCacheAttribute()
        {
            var outputCacheConfigSection = (OutputCacheSection)ConfigurationManager.GetSection("system.web/caching/outputCache");

            if (outputCacheConfigSection.EnableOutputCache)
            {
                IsCachingEnabled = true;

                if (outputCacheConfigSection.DefaultProviderName == AspnetInternalProviderName)
                {
                    Cache = new MemoryCacheProvider();
                }
                else
                {
                    var providerType = Type.GetType(outputCacheConfigSection.Providers[outputCacheConfigSection.DefaultProviderName].Type);
                    Cache = (OutputCacheProvider) Activator.CreateInstance(providerType);
                }
            }
        }        

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ConfigureCacheProfile();

            if (IsCachingEnabled && filterContext.HttpContext.Request.Url != null)
            {
                //todo only VaryByParam = * is supported right now
                _key = string.Concat(CacheKeyPrefix, filterContext.HttpContext.Request.Url.PathAndQuery.ToLower());

                var content = Cache.Get(_key) as string;

                if (content != null)
                {
                    filterContext.Result = new ContentResult { Content = ReplaceDonutHoleContent(content, filterContext) };
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;

            if (viewResult != null)
            {
                using (var stringWriter = new StringWriter())
                {
                    var viewName = String.IsNullOrEmpty(viewResult.ViewName)
                                       ? filterContext.RouteData.GetRequiredString("action")
                                       : viewResult.ViewName;

                    viewResult.View = ViewEngines.Engines.FindView(filterContext, viewName, viewResult.MasterName).View;

                    var viewContext = new ViewContext(filterContext.Controller.ControllerContext, viewResult.View,
                                                      viewResult.ViewData, viewResult.TempData, stringWriter);

                    viewResult.View.Render(viewContext, stringWriter);

                    var content = stringWriter.GetStringBuilder().ToString();

                    if (IsCachingEnabled)
                    {
                        Cache.Add(_key, content, DateTime.Now.AddSeconds(Duration));
                    }

                    filterContext.Result = new ContentResult { Content = ReplaceDonutHoleContent(content, filterContext) };
                }
            }            
        }        

        private void ConfigureCacheProfile()
        {
            if (IsCachingEnabled)
            {
                if (!string.IsNullOrEmpty(CacheProfile))
                {
                    var outputCacheSettingsConfigSection = (OutputCacheSettingsSection)ConfigurationManager.GetSection("system.web/caching/outputCacheSettings");

                    if (outputCacheSettingsConfigSection != null && outputCacheSettingsConfigSection.OutputCacheProfiles.Count > 0)
                    {
                        var cacheProfile = outputCacheSettingsConfigSection.OutputCacheProfiles[CacheProfile];

                        if (cacheProfile != null)
                        {
                            Duration = cacheProfile.Duration;
                            VaryByParam = cacheProfile.VaryByParam;
                            return;
                        }                        
                    }

                    throw new HttpException(string.Format("The '{0}' cache profile is not defined.  Please define it in the configuration file.", CacheProfile));
                }
            }
        }

        private static string ReplaceDonutHoleContent(string content, ControllerContext filterContext)
        {
            return DonutHoles.Replace(content, new MatchEvaluator(match =>
            {
                var actionSettings = new JavaScriptSerializer().Deserialize<ActionSettings>(match.Groups[1].Value);

                return InvokeAction(filterContext.Controller, actionSettings.ActionName, actionSettings.ControllerName, actionSettings.RouteValues);
            }));
        }

        private static string InvokeAction(ControllerBase controller, string actionName, string controllerName, object routeValues)
        {
            var viewContext = new ViewContext(controller.ControllerContext, new WebFormView(controller.ControllerContext, "tmp"),
                                              controller.ViewData, controller.TempData, TextWriter.Null);

            var htmlHelper = new HtmlHelper(viewContext, new ViewPage());

            return htmlHelper.Action(actionName, controllerName, routeValues).ToString();
        }
    }
}