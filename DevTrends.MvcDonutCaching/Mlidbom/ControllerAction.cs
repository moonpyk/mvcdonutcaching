using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class ControllerAction : IControllerAction
    {
        public readonly ActionSettings ActionSettings;
        public readonly ActionDescriptor ActionDescriptor;
        public readonly IDictionary<string, object> ActionParameters;

        public ControllerAction(ActionExecutingContext context)
        {
            Contract.Parameter.NotNull(context);
            ActionSettings = BuildActionSettings(context);
            ActionDescriptor = context.ActionDescriptor;
            ActionParameters = context.ActionParameters;
            if(ActionSettings == null || ActionDescriptor == null || ActionParameters == null)
            {
                throw new Exception("Something that should never be null was null");
            }
        }

        private static ActionSettings BuildActionSettings(ActionExecutingContext context)
        {
            Contract.Parameter.NotNull(context);
            return new ActionSettings()
                   {
                       ActionName = context.ActionDescriptor.ActionName,
                       ControllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName,
                       RouteValues = new RouteValueDictionary(context.ActionParameters)
                   };
        }

        public MvcHtmlString Execute(ControllerContext context)
        {
            Contract.Parameter.NotNull(context);
            var controller = context.Controller;
            var viewContext = new ViewContext(
                controller.ControllerContext,
                new WebFormView(controller.ControllerContext, "tmp"),
                controller.ViewData,
                controller.TempData,
                TextWriter.Null
                );

            var htmlHelper = new HtmlHelper(viewContext, new ViewPage());

            return htmlHelper.Action(
                ActionDescriptor.ActionName,
                ActionDescriptor.ControllerDescriptor.ControllerName,
                new RouteValueDictionary(ActionParameters));
        }

        override public string ToString()
        {
            return string.Format("{0}.{1}(..)",
                ActionDescriptor.ControllerDescriptor.ControllerName,
                ActionDescriptor.ActionName);
        }
    }
}