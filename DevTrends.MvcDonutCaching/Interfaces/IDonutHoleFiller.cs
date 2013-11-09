using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    public interface IDonutHoleFiller
    {
        string RemoveDonutHoleWrappers(string content, ControllerContext filterContext, OutputCacheOptions options);
        string ReplaceDonutHoleContent(string content, ControllerContext filterContext, OutputCacheOptions options);
    }
}
