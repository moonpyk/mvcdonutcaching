using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public ActionResult ClearCache()
        {
            OutputCacheManager.RemoveItems();
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
