using System.Web.Mvc;
using Autofac;
using DevTrends.MvcDonutCaching.Annotations;

namespace DevTrends.MvcDonutCaching.Demo.Mvc
{
    public abstract class ApplicationController : Controller
    {
        public ILifetimeScope LifetimeScope
        {
            get;
            set;
        }

        public OutputCacheManager OutputCacheManager
        {
            get;
            [UsedImplicitly]
            set;
        }
    }
}
