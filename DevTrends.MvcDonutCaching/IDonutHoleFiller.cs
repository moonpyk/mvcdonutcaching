using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    public interface IDonutHoleFiller
    {
        string ReplaceDonutHoleContent(string content, ControllerContext filterContext);
    }
}
