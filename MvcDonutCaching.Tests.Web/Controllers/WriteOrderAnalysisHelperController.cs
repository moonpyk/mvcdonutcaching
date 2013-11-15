using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = 3600)]
    public class WriteOrderAnalysisHelperController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Level1()
        {
            return View();
        }

        public ActionResult Level2()
        {
            return View();
        }
    }
}
