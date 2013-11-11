using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    public class FiveLevelsNestedDonutsController : Controller
    {
        [DonutOutputCache(Duration = 5, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly, DonutOutputCache(Duration = 4, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level1()
        {
            return View();
        }

        [ChildActionOnly, DonutOutputCache(Duration = 3, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level2()
        {
            return View();
        }

        [ChildActionOnly, DonutOutputCache(Duration = 2, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level3()
        {
            return View();
        }

        [ChildActionOnly, DonutOutputCache(Duration = 1, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level4()
        {
            return View();
        }

        [ChildActionOnly, DonutOutputCache(Duration = 0, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level5()
        {
            return View();
        }

    }
}
