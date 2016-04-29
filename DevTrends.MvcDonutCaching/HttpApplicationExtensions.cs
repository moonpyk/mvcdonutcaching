using System;
using System.Web;

namespace DevTrends.MvcDonutCaching
{
    public static class HttpApplicationExtensions
    {
        public const string SkipByCustomApplicationStateKey = "DonutCachingSkipByCustomKey";

        public static void SetSkipByCustomStringDelegate(this HttpApplication application, Func<HttpContextBase, string, bool> func)
        {
            try
            {
                application.Application.Lock();
                application.Application[SkipByCustomApplicationStateKey] = func;
            }
            finally
            {
                application.Application.UnLock();
            }
        }

        public static Func<HttpContext, string, bool> GetSkipByCustomDelegate(this HttpApplication application)
        {
            return application.Application[SkipByCustomApplicationStateKey] as Func<HttpContext, string, bool>;
        }
    }
}