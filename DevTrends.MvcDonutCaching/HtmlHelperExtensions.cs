using DevTrends.MvcDonutCaching.Annotations;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public static class HtmlHelperExtensions
    {
        private static IActionSettingsSerialiser _serialiser;

        /// <summary>
        /// Gets or sets the serialiser.
        /// </summary>
        /// <value>
        /// The serialiser.
        /// </value>
        public static IActionSettingsSerialiser Serialiser
        {
            get
            {
                return _serialiser ??  (_serialiser = new EncryptingActionSettingsSerialiser(new ActionSettingsSerialiser(), new Encryptor()));
            }
            set
            {
                _serialiser = value;
            }
        }

        /// <summary>
        /// Invokes the specified child action method and returns the result as an HTML string.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper h, [AspMvcAction] string actionName, bool excludeFromParentCache)
        {
            return h.Action(actionName, null, null, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and returns the result as an HTML string.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper h, [AspMvcAction] string actionName, object routeValues, bool excludeFromParentCache)
        {
            return h.Action(actionName, null, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and returns the result as an HTML string.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper h, [AspMvcAction] string actionName, RouteValueDictionary routeValues, bool excludeFromParentCache)
        {
            return h.Action(actionName, null, routeValues, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper h, [AspMvcAction] string actionName, [AspMvcController] string controllerName, bool excludeFromParentCache)
        {
            return h.Action(actionName, controllerName, null, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper h, [AspMvcAction] string actionName, [AspMvcController] string controllerName, object routeValues, bool excludeFromParentCache)
        {
            return h.Action(actionName, controllerName, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the child action method to invoke.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>

        public static void RenderAction(this HtmlHelper h, [AspMvcAction] string actionName, bool excludeFromParentCache)
        {
            RenderAction(h, actionName, null, null, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the child action method to invoke.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>

        public static void RenderAction(this HtmlHelper h, [AspMvcAction] string actionName, object routeValues, bool excludeFromParentCache)
        {
            RenderAction(h, actionName, null, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the child action method to invoke.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>        
        public static void RenderAction(this HtmlHelper h, [AspMvcAction] string actionName, RouteValueDictionary routeValues, bool excludeFromParentCache)
        {
            RenderAction(h, actionName, null, routeValues, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the child action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>        
        public static void RenderAction(this HtmlHelper h, [AspMvcAction] string actionName, [AspMvcController] string controllerName, bool excludeFromParentCache)
        {
            RenderAction(h, actionName, controllerName, null, excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the child action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        public static void RenderAction(this HtmlHelper h, [AspMvcAction] string actionName, [AspMvcController] string controllerName, object routeValues, bool excludeFromParentCache)
        {
            RenderAction(h, actionName, controllerName, new RouteValueDictionary(routeValues), excludeFromParentCache);
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the child action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        public static void RenderAction(this HtmlHelper h, [AspMvcAction] string actionName, [AspMvcController] string controllerName, RouteValueDictionary routeValues, bool excludeFromParentCache)
        {
            if (excludeFromParentCache)
            {
                var serialisedActionSettings = GetSerialisedActionSettings(actionName, controllerName, routeValues);

                h.ViewContext.Writer.Write("<!--Donut#{0}#-->", serialisedActionSettings);
            }

            h.RenderAction(actionName, controllerName, routeValues);

            if (excludeFromParentCache)
            {
                h.ViewContext.Writer.Write("<!--EndDonut-->");
            }
        }

        /// <summary>
        /// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
        /// </summary>
        /// <param name="h">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method to invoke.</param>
        /// <param name="controllerName">The name of the controller that contains the action method.</param>
        /// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
        /// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
        /// <returns>The child action result as an HTML string.</returns>
        public static MvcHtmlString Action(this HtmlHelper h, [AspMvcAction] string actionName, [AspMvcController] string controllerName, RouteValueDictionary routeValues, bool excludeFromParentCache)
        {
            if (excludeFromParentCache)
            {
                var serialisedActionSettings = GetSerialisedActionSettings(actionName, controllerName, routeValues);

                return new MvcHtmlString(string.Format("<!--Donut#{0}#-->{1}<!--EndDonut-->", serialisedActionSettings, h.Action(actionName, controllerName, routeValues)));
            }

            return h.Action(actionName, controllerName, routeValues);
        }

        private static string GetSerialisedActionSettings(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            var actionSettings = new ActionSettings
            {
                ActionName = actionName,
                ControllerName = controllerName,
                RouteValues = routeValues
            };

            return Serialiser.Serialise(actionSettings);
        }
    }
}
