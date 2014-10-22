using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Demo.Mvc;
using Newtonsoft.Json;

namespace DevTrends.MvcDonutCaching.Demo.Controllers
{
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Simple");
        }

        //
        // GET: /Home/
        [DonutOutputCache(Duration = 24 * 3600)]
        public ActionResult Simple()
        {
            return View(DateTime.Now);
        }

        [ChildActionOnly, DonutOutputCache(Duration = 60, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult SimpleDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly, DonutOutputCache(Duration = 5)]
        public ActionResult NestedDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly]
        public ActionResult SimpleDonutTwo()
        {
            return PartialView(DateTime.Now);
        }

        public ActionResult ExpireSimpleDonutCache()
        {
            OutputCacheManager.RemoveItem("Home", "Simple");

            return Content("OK", "text/plain");
        }

        public ActionResult ExpireSimpleDonutOneCache()
        {
            OutputCacheManager.RemoveItem("Home", "SimpleDonutOne");

            return Content("OK", "text/plain");
        }

        [DonutOutputCache(CacheProfile = "medium", VaryByParam = "*", VaryByCustom = "subdomain")]
        public ActionResult TestIssue23()
        {
            return View();
        }

        [DonutOutputCache(Duration = 3600 /* Bacon is still good one hour later */)]
        public async Task<ActionResult> WorksOnAsyncMethodsToo()
        {
            var req = WebRequest.Create("http://baconipsum.com/api/?type=meat-and-filler");

            string[] final = null;

            using (var resp = await req.GetResponseAsync())
            {
                var rStream = resp.GetResponseStream();
                if (rStream != null)
                {
                    using (var r = new StreamReader(rStream))
                    {
                        final = JsonConvert.DeserializeObject<string[]>(r.ReadToEnd());
                    }
                }
            }

            return View(final);
        }
    }
}
