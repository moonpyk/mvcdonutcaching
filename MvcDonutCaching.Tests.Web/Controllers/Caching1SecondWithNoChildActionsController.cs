using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [DonutOutputCache(Duration = .1)]
    public class Caching1SecondWithNoChildActionsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
