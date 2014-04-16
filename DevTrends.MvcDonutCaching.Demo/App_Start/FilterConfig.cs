using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace DevTrends.MvcDonutCaching.Demo
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