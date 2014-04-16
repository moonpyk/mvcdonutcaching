using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    public class MultipleIdenticalChildActionCallsController : Controller
    { 
        [AutoOutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View();
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult Child()
        {
            return View();
        }
    }
}