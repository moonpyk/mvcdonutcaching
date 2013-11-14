using System;
using System.IO;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AutoOutputCacheAttribute : ActionFilterAttribute
    {
        private TextWriter _oldOutput;
        private string _action;
        private StringWriter _myOutPut;

        public AutoOutputCacheAttribute()
        {
            Order = int.MaxValue;
            bool breakpoint = true;
        }
        override public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DonutOutputManager.ActionExecuting(filterContext);
        }

        override public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        override public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        override public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            DonutOutputManager.ResultExecuted(filterContext);
        }
    }
}