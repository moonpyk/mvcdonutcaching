using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using DevTrends.MvcDonutCaching.Mlidbom;
using MvcDonutCaching.Tests.Web.Models;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = .5)]
    public class FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManagedController : Controller
    {
        [AutoOutputCache(Duration = .5, ExcludeFromParentCache = true)]
        public ActionResult Index()
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = "Level0" });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .4, ExcludeFromParentCache = true)]
        public ActionResult Level1(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .3, ExcludeFromParentCache = true)]
        public ActionResult Level2(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .2, ExcludeFromParentCache = true)]
        public ActionResult Level3(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .1, ExcludeFromParentCache = true)]
        public ActionResult Level4(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = 0, ExcludeFromParentCache = true)]
        public ActionResult Level5(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

    }
}
