using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using MvcDonutCaching.Tests.Web.Models;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    public class FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManagedController : Controller
    {
        [DonutOutputCache(Duration = 5, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Index()
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = "Level0" });
        }

        [ChildActionOnly, ExcludeFromParentCache(Duration = 4, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level1(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, ExcludeFromParentCache(Duration = 3, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level2(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, ExcludeFromParentCache(Duration = 2, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level3(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, ExcludeFromParentCache(Duration = 1, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level4(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, ExcludeFromParentCache(Duration = 0, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level5(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

    }
}
