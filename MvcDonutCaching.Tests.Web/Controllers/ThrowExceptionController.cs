using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;
using MvcDonutCaching.Tests.Web.Models;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = 3600)]
    public class ThrowExceptionController : Controller
    {
        public ActionResult ThrowNow()
        {
            return View();
        }

        public ActionResult ThrowInNextLevel()
        {
            return View();
        }

        public ActionResult ThrowTwoLevelsDown()
        {
            return View();
        }
    }
}
