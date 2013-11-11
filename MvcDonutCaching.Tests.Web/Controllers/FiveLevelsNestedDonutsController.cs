using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using MvcDonutCaching.Tests.Web.Models;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    public class FiveLevelsNestedDonutsController : Controller
    {
        [DonutOutputCache(Duration = .5, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Index()
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = "Level0" });
        }

        [ChildActionOnly, DonutOutputCache(Duration = .4, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level1(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, DonutOutputCache(Duration = .3, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level2(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, DonutOutputCache(Duration = .2, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level3(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, DonutOutputCache(Duration = .1, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level4(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, DonutOutputCache(Duration = 0, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult Level5(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

    }
}
