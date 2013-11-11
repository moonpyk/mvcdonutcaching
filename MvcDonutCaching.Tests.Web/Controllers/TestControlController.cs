using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    public class TestControlController : Controller
    {
        //
        // GET: /TestControl/

        public ActionResult ClearCache(DateTime time)
        {
            return Content("Done");
        }

    }
}
