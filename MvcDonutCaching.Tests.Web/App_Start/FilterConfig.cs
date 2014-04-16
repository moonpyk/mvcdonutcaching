using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace MvcDonutCaching.Tests.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            FilterProviders.Providers.Add(new DefaultAutoOutputCacheFilterProvider());
        }
    }
}