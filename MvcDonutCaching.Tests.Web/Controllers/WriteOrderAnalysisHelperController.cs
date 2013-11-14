using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using MvcDonutCaching.Tests.Web.Models;

namespace MvcDonutCaching.Tests.Web.Controllers
{
    [WriteAnalysis]
    public class WriteOrderAnalysisHelperController : Controller
    {
        public ActionResult Root()
        {
            return View();
        }

        public ActionResult Level1()
        {
            return View();
        }

        public ActionResult Level2()
        {
            return View();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class WriteAnalysisAttribute : ActionFilterAttribute
    {
        private TextWriter _oldOutput;
        private string _action;
        private StringWriter _myOutPut;

        public WriteAnalysisAttribute()
        {
            Order = int.MaxValue;
            bool breakpoint = true;
        }
        override public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _action = filterContext.ActionDescriptor.ActionName;
            if(_oldOutput != null)
            {
                throw new Exception("huh?");
            }
            _oldOutput = filterContext.HttpContext.Response.Output;
            filterContext.HttpContext.Response.Output = _myOutPut = new StringWriter(CultureInfo.InvariantCulture);
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
            if (_oldOutput == null)
            {
                throw new Exception("huh?");
            }
            if(!ReferenceEquals(filterContext.HttpContext.Response.Output, _myOutPut))
            {
                throw new Exception("huh?");
            }
            _oldOutput.Write(filterContext.HttpContext.Response.Output.ToString());
            filterContext.HttpContext.Response.Output = _oldOutput;
        }
    }
}
