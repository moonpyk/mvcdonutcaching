using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using DevTrends.MvcDonutCaching.Annotations;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    public class TestControlController : Controller
    {
        public OutputCacheManager OutputCacheManager
        {
            get;
            [UsedImplicitly]
            set;
        }

        public ActionResult ClearCache(string controllerName = null, string actionName = null)
        {
            OutputCacheManager.RemoveItems(controllerName, actionName);
            return Content("Done");
        }

        public ActionResult EnableReplaceDonutsInChildActionsGlobally()
        {
            OutputCache.DefaultOptions = OutputCacheOptions.ReplaceDonutsInChildActions;
            return Content("Done");
        }

        public ActionResult UseDefaultSettingsGlobally()
        {
            OutputCache.DefaultOptions = OutputCacheOptions.None;
            return Content("Done");
        }

    }
}
