using System;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AutoOutputCacheAttribute : ActionFilterAttribute
    {
        public AutoOutputCacheAttribute()
        {
        }
        override public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DonutOutputManager.ActionExecuting(filterContext);
        }

        override public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            DonutOutputManager.ResultExecuted(filterContext);
        }
    }
}