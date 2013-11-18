using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public static class ControllerActionExecutor 
    {
        public static MvcHtmlString Execute(this ControllerAction me, ControllerContext context)
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
                 me.ActionName,
                me.ControllerName,
                new RouteValueDictionary(me.ActionParameters));
        }
    }
}