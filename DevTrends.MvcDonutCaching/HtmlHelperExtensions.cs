using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public static class HtmlHelperExtensions
    {
        private static IActionSettingsSerialiser _serialiser = new EncryptingActionSettingsSerialiser(new ActionSettingsSerialiser(), new Encryptor());

        /// <summary>
        /// Invokes the specified child action method and returns the result as an HTML string.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper html, string actionName, bool excludeFromParentCache)
        {
            return html.Action(actionName, null, null, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and returns the result as an HTML string.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper html, string actionName, object routeValues, bool excludeFromParentCache)
        {
            return html.Action(actionName, null, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and returns the result as an HTML string.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper html, string actionName, RouteValueDictionary routeValues, bool excludeFromParentCache)
        {
            return html.Action(actionName, null, routeValues, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper html, string actionName, string controllerName, bool excludeFromParentCache)
        {
            return html.Action(actionName, controllerName, null, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper html, string actionName, string controllerName, object routeValues, bool excludeFromParentCache)
        {
            return html.Action(actionName, controllerName, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
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

                var serialisedActionSettings = _serialiser.Serialise(actionSettings);

                return new MvcHtmlString(string.Format("<!--Donut#{0}#-->{1}<!--EndDonut-->", serialisedActionSettings, html.Action(actionName, controllerName, routeValues)));                
            }

            return html.Action(actionName, controllerName, routeValues);
        }        
    }
}