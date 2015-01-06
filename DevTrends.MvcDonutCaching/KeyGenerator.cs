using DevTrends.MvcDonutCaching.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public class KeyGenerator : IKeyGenerator
    {
        private const string ROUTE_DATA_KEY_ACTION = "action";
        private const string ROUTE_DATA_KEY_CONTROLLER = "controller";
        private const string DATA_TOKENS_KEY_AREA = "area";

        private readonly IKeyBuilder _keyBuilder;

        public KeyGenerator(IKeyBuilder keyBuilder)
        {
            if (keyBuilder == null) { throw new ArgumentNullException("keyBuilder"); }

            _keyBuilder = keyBuilder;
        }

        [CanBeNull]
        public string GenerateKey(ControllerContext context, CacheSettings cacheSettings)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (cacheSettings == null) { throw new ArgumentNullException("cacheSettings"); }

            RouteData routeData = context.RouteData;

            if (routeData == null) { return null; }

            string actionName = GetActionNameFromRouteData(routeData);
            string controllerName = GetControllerNameFromRouteData(routeData);
            
            if (string.IsNullOrEmpty(actionName) || 
                string.IsNullOrEmpty(controllerName))
            {
                return null;
            }

            string areaName = GetAreaNameFromRouteData(routeData);

            // remove controller, action and DictionaryValueProvider which is added by the framework for child actions
            var filteredRouteData = routeData.Values.Where(
                x => x.Key.ToLowerInvariant() != ROUTE_DATA_KEY_CONTROLLER && 
                     x.Key.ToLowerInvariant() != ROUTE_DATA_KEY_ACTION &&   
                     x.Key.ToLowerInvariant() != DATA_TOKENS_KEY_AREA &&
                     !(x.Value is DictionaryValueProvider<object>)
            ).ToList();

            if (!string.IsNullOrWhiteSpace(areaName))
            {
                filteredRouteData.Add(new KeyValuePair<string, object>(DATA_TOKENS_KEY_AREA, areaName));
            }

            var routeValues = new RouteValueDictionary(filteredRouteData.ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value));

            if (!context.IsChildAction)
            {
                // note that route values take priority over form values and form values take priority over query string values
                if ((cacheSettings.Options & OutputCacheOptions.IgnoreFormData) != OutputCacheOptions.IgnoreFormData)
                {
                    foreach (var formKey in context.HttpContext.Request.Form.AllKeys)
                    {
                        if (routeValues.ContainsKey(formKey.ToLowerInvariant()))
                        {
                            continue;
                        }

                        var item = context.HttpContext.Request.Form[formKey];
                        routeValues.Add(
                            formKey.ToLowerInvariant(),
                            item != null 
                                ? item.ToLowerInvariant() 
                                : string.Empty
                        );
                    }
                }

                if ((cacheSettings.Options & OutputCacheOptions.IgnoreQueryString) != OutputCacheOptions.IgnoreQueryString)
                {
                    foreach (var queryStringKey in context.HttpContext.Request.QueryString.AllKeys)
                    {
                        // queryStringKey is null if url has qs name without value. e.g. test.com?q
                        if (queryStringKey == null || routeValues.ContainsKey(queryStringKey.ToLowerInvariant()))
                        {
                            continue;
                        }

                        var item = context.HttpContext.Request.QueryString[queryStringKey];
                        routeValues.Add(
                            queryStringKey.ToLowerInvariant(),
                            item != null 
                                ? item.ToLowerInvariant() 
                                : string.Empty
                        );
                    }
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
                // If there is an existing route value with the same key as varybycustom, we should overwrite it
                routeValues[cacheSettings.VaryByCustom.ToLowerInvariant()] =
                            context.HttpContext.ApplicationInstance.GetVaryByCustomString(HttpContext.Current, cacheSettings.VaryByCustom);
            }

            var key = _keyBuilder.BuildKey(controllerName, actionName, routeValues);

            return key;
        }

        private static string GetActionNameFromRouteData(RouteData routeData)
        {
            string actionName = null;

            if (routeData.Values.ContainsKey(ROUTE_DATA_KEY_ACTION) &&
                routeData.Values[ROUTE_DATA_KEY_ACTION] != null)
            {
                actionName = routeData.Values[ROUTE_DATA_KEY_ACTION].ToString();
            }

            return actionName;
        }

        private static string GetControllerNameFromRouteData(RouteData routeData)
        {
            string controllerName = null;

            if (routeData.Values.ContainsKey(ROUTE_DATA_KEY_CONTROLLER) &&
                routeData.Values[ROUTE_DATA_KEY_CONTROLLER] != null)
            {
                controllerName = routeData.Values[ROUTE_DATA_KEY_CONTROLLER].ToString();
            }

            return controllerName;
        }

        private static string GetAreaNameFromRouteData(RouteData routeData)
        {
            string areaName = null;

            if (routeData.DataTokens.ContainsKey(DATA_TOKENS_KEY_AREA))
            {
                areaName = routeData.DataTokens[DATA_TOKENS_KEY_AREA].ToString();
            }

            return areaName;
        }
    }
}
