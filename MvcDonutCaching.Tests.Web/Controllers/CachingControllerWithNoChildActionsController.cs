using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    public class CachingControllerWithNoChildActionsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
