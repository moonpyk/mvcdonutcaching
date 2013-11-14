using System.Web.Mvc;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache]
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
