using System;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Simple");
        }

        //
        // GET: /Home/
        [DonutOutputCache(Duration = 24 * 3600)]
        //[OutputCache(Duration = 24 * 3600)] //ez tönkreteszi a hierarchiát, mindent egyben cache-el
        public ActionResult Simple()
        {
            return View(DateTime.Now);
        }

        [ChildActionOnly, DonutOutputCache(Duration = 60)]
        public ActionResult SimpleDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly]
        public ActionResult SimpleDonutTwo()
        {
            return PartialView(DateTime.Now);
        }

        public void ExpireCache()
        {
            //Ez AAAA módszer a jó
            var cacheManager = new OutputCacheManager();
            cacheManager.RemoveItem("Home", "SimpleDonutOne");
        }

        public void ExpireCache2()
        {
            //Rossz módszer, nem működik, a teljes cache-t kiüti!
            ((MemoryCache)OutputCacheAttribute.ChildActionCache).Dispose();
            OutputCacheAttribute.ChildActionCache = new MemoryCache("NewDefault");
        }
    }
}
