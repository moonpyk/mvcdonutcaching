using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IControllerAction
    {
        MvcHtmlString Execute(ControllerContext context);
    }
}
