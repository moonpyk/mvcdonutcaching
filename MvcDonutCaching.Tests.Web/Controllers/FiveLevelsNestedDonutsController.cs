using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;
using MvcDonutCaching.Tests.Web.Models;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = .5)]
    public class FiveLevelsNestedDonutsController : Controller
    {
        [AutoOutputCache(Duration = .0)]
        public ActionResult Index()
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = "Level0" });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .1)]
        public ActionResult Level1(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .2)]
        public ActionResult Level2(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .3)]
        public ActionResult Level3(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .4)]
        public ActionResult Level4(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

        [ChildActionOnly, AutoOutputCache(Duration = .5)]
        public ActionResult Level5(DateTime time, string title)
        {
            return View(new TitleAndTime { Time = DateTime.Now.ToString("o"), Title = title });
        }

    }
}
