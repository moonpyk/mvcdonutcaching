using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Demo.Lib;

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
        [ProtoBufDonutCache(Duration = 24 * 3600)]
        public ActionResult Simple()
        {
            return View(DateTime.Now);
        }

        [ChildActionOnly, ProtoBufDonutCache(Duration = 60)]
        public ActionResult SimpleDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly, ProtoBufDonutCache(Duration = 5)]
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
