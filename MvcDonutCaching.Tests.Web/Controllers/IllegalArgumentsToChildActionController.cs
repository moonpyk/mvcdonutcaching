using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [AutoOutputCache(Duration = 3600)]
    public class IllegalArgumentsToChildActionController : Controller
    {
        public ActionResult IllegalArgumentsInRoot()
        {
            return View();
        }

        public ActionResult IllegalArgumentsInFirstChild()
        {
            return View();
        }

        public ActionResult IllegalArgumentsInNestedChild()
        {
            return View();
        }

        public ActionResult ChildActionRequiringParameter(int theParameter)
        {
            return View();
        }
    }
}