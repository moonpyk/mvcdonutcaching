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
            OutputCacheManager.RemoveItem("Home", "CachedHeaders");

            return Content("OK", "text/plain");
        }

        public ActionResult ExpireSimpleDonutOneCache()
        {
            OutputCacheManager.RemoveItem("Home", "SimpleDonutOne");

            return Content("OK", "text/plain");
        }

        [DonutOutputCache(CacheProfile = "medium", VaryByParam = "*", VaryByCustom = "subdomain")]
        public ActionResult TestIssue23()
        {
            return View();
        }


        [DonutOutputCache(Duration = 60, CachedHeaders = "x-cache-me")]
        public ActionResult CachedHeaders() {
            Response.AppendHeader("x-cache-me", Guid.NewGuid().ToString());
            return Json(new {
                CacheTime = DateTime.Now
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
