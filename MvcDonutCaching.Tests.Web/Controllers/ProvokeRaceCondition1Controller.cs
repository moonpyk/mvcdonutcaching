using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = 0)]
    public class ProvokeRaceCondition1Controller : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }
        
        [AutoOutputCache(Duration = 3600)]
        public ActionResult TopLayoutHeader()
        {
            return View();
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult TopLayoutFooter()
        {
            return View();
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult NestedLayoutHeader()
        {
            return View();
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult NestedLayoutFooter()
        {
            return View();
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult PageBody()
        {
            return View();
        }

    }
}
