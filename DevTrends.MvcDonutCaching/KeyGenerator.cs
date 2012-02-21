using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public class KeyGenerator : IKeyGenerator
    {
        private readonly IKeyBuilder _keyBuilder;

        public KeyGenerator(IKeyBuilder keyBuilder)
        {
            if (keyBuilder == null)
            {
                throw new ArgumentNullException("keyBuilder");
            }

            _keyBuilder = keyBuilder;
        }

        public string GenerateKey(ControllerContext context, CacheSettings cacheSettings)
        {
            var actionName = context.RouteData.Values["action"].ToString();
            var controllerName = context.RouteData.Values["controller"].ToString();

            // remove controller, action and DictionaryValueProvider which is added by the framework for child actions
            var filteredRouteData = context.RouteData.Values.Where(x => x.Key.ToLowerInvariant() != "controller" && 
                                                                   x.Key.ToLowerInvariant() != "action" && 
                                                                   !(x.Value is DictionaryValueProvider<object>));

            var routeValues = new RouteValueDictionary(filteredRouteData.ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value));

            if (!context.IsChildAction)
            {
                foreach (var queryStringKey in context.HttpContext.Request.QueryString.AllKeys)
                {
                    routeValues.Add(queryStringKey.ToLowerInvariant(), context.HttpContext.Request.QueryString[queryStringKey].ToLowerInvariant());
                }

                foreach (var formKey in context.HttpContext.Request.Form.AllKeys)
                {
                    routeValues.Add(formKey.ToLowerInvariant(), context.HttpContext.Request.Form[formKey].ToLowerInvariant());
                }
            }

            if (!string.IsNullOrEmpty(cacheSettings.VaryByParam))
            {
                if (cacheSettings.VaryByParam.ToLowerInvariant() == "none")
                {
                    routeValues.Clear();
                }
                else if (cacheSettings.VaryByParam != "*")
                {
                    var parameters = cacheSettings.VaryByParam.ToLowerInvariant().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    routeValues = new RouteValueDictionary(routeValues.Where(x => parameters.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value));
                }
            }

            if (!string.IsNullOrEmpty(cacheSettings.VaryByCustom))
            {
                routeValues.Add(cacheSettings.VaryByCustom.ToLowerInvariant(), context.HttpContext.ApplicationInstance.GetVaryByCustomString(HttpContext.Current, cacheSettings.VaryByCustom));
            }

            var key = _keyBuilder.BuildKey(controllerName, actionName, routeValues);

            return key;
        }
    }
}
