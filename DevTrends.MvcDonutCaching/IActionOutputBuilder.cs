using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    public interface IActionOutputBuilder
    {
        string GetActionOutput(ViewResultBase viewResult, ActionExecutedContext filterContext);
    }
}
