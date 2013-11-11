using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [DonutOutputCache(Duration = .1)]
    public class Caching1SecondControllerWithNoChildActionsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
