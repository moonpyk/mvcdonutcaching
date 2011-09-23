using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace DevTrends.MvcDonutCaching
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Action(this HtmlHelper html, string actionName, bool excludeFromParentCache)
        {
            return html.Action(actionName, null, null, excludeFromParentCache);
        }

        public static MvcHtmlString Action(this HtmlHelper html, string actionName, object routeValues, bool excludeFromParentCache)
        {
            return html.Action(actionName, null, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        public static MvcHtmlString Action(this HtmlHelper html, string actionName, RouteValueDictionary routeValues, bool excludeFromParentCache)
        {
            return html.Action(actionName, null, routeValues, excludeFromParentCache);
        }

        public static MvcHtmlString Action(this HtmlHelper html, string actionName, string controllerName, bool excludeFromParentCache)
        {
            return html.Action(actionName, controllerName, null, excludeFromParentCache);
        }

        public static MvcHtmlString Action(this HtmlHelper html, string actionName, string controllerName, object routeValues, bool excludeFromParentCache)
        {
            return html.Action(actionName, controllerName, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        public static MvcHtmlString Action(this HtmlHelper html, string actionName, string controllerName, RouteValueDictionary routeValues, bool excludeFromParentCache)
        {
            if (excludeFromParentCache)
            {
                var actionSettings = new ActionSettings
                {
                    ActionName = actionName,
                    ControllerName = controllerName,
                    RouteValues = routeValues
                };

                var serialisedActionSettings = new JavaScriptSerializer().Serialize(actionSettings);

                return new MvcHtmlString(string.Format("<!--Donut#{0}#-->", serialisedActionSettings));                
            }

            return html.Action(actionName, controllerName, routeValues);
        }
    }
}