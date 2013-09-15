using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Demo.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [DonutOutputCache(Duration = 24 * 3600)]
        public ActionResult Index()
        {
            return View(DateTime.Now);
        }
    }
}
