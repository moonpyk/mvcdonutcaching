using JetBrains.Annotations;
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
        internal const string RouteDataKeyAction     = "action";
        internal const string RouteDataKeyController = "controller";
        internal const string DataTokensKeyArea      = "area";

        private readonly IKeyBuilder _keyBuilder;

        public KeyGenerator(IKeyBuilder keyBuilder)
        {
            if (keyBuilder == null)
            {
                throw new ArgumentNullException("keyBuilder");
            }

            _keyBuilder = keyBuilder;
        }

        /// <summary>
        /// Generates a key given the <see cref="context"/> and <see cref="cacheSettings"/>.
        /// </summary>
        /// <param name="context">The controller context.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        /// <returns>A string that can be used as an output cache key</returns>
        [CanBeNull]
        public string GenerateKey(ControllerContext context, CacheSettings cacheSettings)
        {
            var routeData = context.RouteData;

            if (routeData == null)
            {
                return null;
            }

            string actionName = null,
                controllerName = null;

            if (
                routeData.Values.ContainsKey(RouteDataKeyAction) &&
                routeData.Values[RouteDataKeyAction] != null)
            {
                actionName = routeData.Values[RouteDataKeyAction].ToString();
            }

            if (
                routeData.Values.ContainsKey(RouteDataKeyController) && 
                routeData.Values[RouteDataKeyController] != null)
            {
                controllerName = routeData.Values[RouteDataKeyController].ToString();
            }

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
            {
                return null;
            }

            string areaName = null;

            if (
                routeData.DataTokens.ContainsKey(DataTokensKeyArea) &&
                routeData.DataTokens[DataTokensKeyArea] != null
            )
            {
                areaName = routeData.DataTokens[DataTokensKeyArea].ToString();
            }

            // remove controller, action and DictionaryValueProvider which is added by the framework for child actions
            var filteredRouteData = routeData.Values.Where(
                x => x.Key.ToLowerInvariant() != RouteDataKeyController && 
                     x.Key.ToLowerInvariant() != RouteDataKeyAction &&   
                     x.Key.ToLowerInvariant() != DataTokensKeyArea &&
                     !(x.Value is DictionaryValueProvider<object>)
            ).ToList();

            if (!string.IsNullOrWhiteSpace(areaName))
            {
                filteredRouteData.Add(new KeyValuePair<string, object>(DataTokensKeyArea, areaName));
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
                        // queryStringKey is null if url has as name without value. e.g. test.com?q
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
                    routeValues = new RouteValueDictionary(routeValues.Where(x => parameters.Contains(x.Key))
                        .ToDictionary(x => x.Key, x => x.Value));
                }
            }

            if (!string.IsNullOrEmpty(cacheSettings.VaryByCustom))
            {
                // If there is an existing route value with the same key as varybycustom, we should overwrite it
                routeValues[cacheSettings.VaryByCustom.ToLowerInvariant()] =
                            context.HttpContext.ApplicationInstance.GetVaryByCustomString(HttpContext.Current, cacheSettings.VaryByCustom);
            }

            if (!string.IsNullOrEmpty(cacheSettings.VaryByHeader))
            {
                var headers = cacheSettings.VaryByHeader.ToLowerInvariant().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var existingHeaders = context.HttpContext.Request.Headers.AllKeys.Where(x => headers.Contains(x.ToLowerInvariant()));
                foreach (var header in existingHeaders)
                {
                    routeValues[header] = context.HttpContext.Request.Headers[header];
                }
            }

            var key = _keyBuilder.BuildKey(controllerName, actionName, routeValues);

            return key;
        }
    }
}
