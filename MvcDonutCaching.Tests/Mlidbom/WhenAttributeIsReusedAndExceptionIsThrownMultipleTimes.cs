using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using DevTrends.MvcDonutCaching.Mlidbom;
using Moq;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    public class WhenAttributeIsReusedAndExceptionIsThrownMultipleTimes
    {
        [Test]
        public void OutputIsStillCorrectlyRestored()
        {
            var context = TestUtil.CreateMockActionExecutingControllerContext();

            var attribute = new AutoOutputCacheAttribute()
                            {
                                Duration = 3600
                            };

            var correctOutput = context.HttpContext.Response.Output;
            correctOutput.Write("correct output");

            context.RouteData.Values["action"] = "action";
            context.RouteData.Values["controller"] = "controller";

            context.ActionDescriptor = new StaticActionDescriptor("controller", "action");

            var exceptionContext = new ExceptionContext();
            exceptionContext.HttpContext = context.HttpContext;

            //simulate one failure
            attribute.OnActionExecuting(context);
            attribute.OnException(exceptionContext);
            Assert.That(context.HttpContext.Response.Output, Is.SameAs(correctOutput));

            //simulate another failure
            attribute.OnActionExecuting(context);//Pop one action onto the stack                       
            attribute.OnException(exceptionContext);
            Assert.That(context.HttpContext.Response.Output, Is.SameAs(correctOutput));

        }
    }
}