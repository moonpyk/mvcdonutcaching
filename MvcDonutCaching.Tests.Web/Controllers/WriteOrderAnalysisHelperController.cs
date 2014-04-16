using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = 3600)]
    public class WriteOrderAnalysisHelperController : Controller
    {
        [AutoOutputCache(Duration = 2)]
        public ActionResult Index()
        {
            return View();
        }

        [AutoOutputCache(Duration = 4)]
        public ActionResult Level1()
        {
            return View();
        }

        [AutoOutputCache(Duration = 6)]
        public ActionResult Level2()
        {
            return View();
        }
    }
}
