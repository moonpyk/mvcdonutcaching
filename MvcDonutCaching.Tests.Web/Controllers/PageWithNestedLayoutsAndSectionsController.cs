using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = 3600)]
    public class PageWithNestedLayoutsAndSectionsController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult TopLayoutHeader()
        {
            return View();
        }

        public ActionResult TopLayoutFooter()
        {
            return View();
        }

        public ActionResult NestedLayoutHeader()
        {
            return View();
        }

        public ActionResult NestedLayoutFooter()
        {
            return View();
        }

        public ActionResult PageBody()
        {
            return View();
        }

    }
}
