using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Demo.Controllers;
using DevTrends.MvcDonutCaching.Demo.Mvc;

namespace DevTrends.MvcDonutCaching.Demo.Areas.SubArea.Controllers
{
    public class SubHomeController : ApplicationController
    {
        //
        // GET: /SubArea/Home/
        [DonutOutputCache(Duration = 24 * 3600)]
        public ActionResult Index()
        {
            return View(DateTime.Now);
        }

        [ChildActionOnly, DonutOutputCache(Duration = 30, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult AreaDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        public ActionResult ExpireAreaDonutOne()
        {
            OutputCacheManager.RemoveItem("SubHome", "AreaDonutOne", new { area="SubArea" });

            return Content("OK", "text/plain");
        }

        public ActionResult ExpireAreaDonuts()
        {
            OutputCacheManager.RemoveItems(null, null, new { area="SubArea" });

            return Content("OK", "text/plain");
        }

    }
}
