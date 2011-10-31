using System;
using System.IO;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    public class ActionOutputBuilder : IActionOutputBuilder
    {
        public string GetActionOutput(ViewResultBase viewResult, ActionExecutedContext filterContext)
        {
            var viewName = String.IsNullOrEmpty(viewResult.ViewName)
                                ? filterContext.RouteData.GetRequiredString("action")
                                : viewResult.ViewName;

            var fullViewResult = viewResult as ViewResult;

            if (fullViewResult == null)
            {
                viewResult.View = ViewEngines.Engines.FindPartialView(filterContext, viewName).View;
            }
            else
            {
                viewResult.View = ViewEngines.Engines.FindView(filterContext, viewName, fullViewResult.MasterName).View;
            }

            using (var stringWriter = new StringWriter())
            {
                var viewContext = new ViewContext(filterContext.Controller.ControllerContext, viewResult.View,
                                                  viewResult.ViewData, viewResult.TempData, stringWriter);

                viewResult.View.Render(viewContext, stringWriter);

                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
