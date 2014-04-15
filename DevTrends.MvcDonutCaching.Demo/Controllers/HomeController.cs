using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Demo.Mvc;

namespace DevTrends.MvcDonutCaching.Demo.Controllers
{
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Simple");
        }

        //
        // GET: /Home/
        [DonutOutputCache(Duration = 24 * 3600)]
        public ActionResult Simple()
        {
            return View(DateTime.Now);
        }

        [ChildActionOnly, DonutOutputCache(Duration = 60, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult SimpleDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly, DonutOutputCache(Duration = 5)]
        public ActionResult NestedDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly]
        public ActionResult SimpleDonutTwo()
        {
            return PartialView(DateTime.Now);
        }

        public ActionResult ExpireSimpleDonutCache()
        {
            OutputCacheManager.RemoveItem("Home", "Simple");

            return Content("OK", "text/plain");
        }

        public ActionResult ExpireSimpleDonutOneCache()
        {
            OutputCacheManager.RemoveItem("Home", "SimpleDonutOne");

            return Content("OK", "text/plain");
        }
    }
}
