using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using DevTrends.MvcDonutCaching.Mlidbom;
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

            var filter = new AutoOutputCacheFilter(
                new KeyGenerator(new KeyBuilder()),
                new OutputCacheManager(OutputCache.Instance, new KeyBuilder()),
                new CacheSettingsManager())
                         {
                             Config = attribute
                         };

            var correctOutput = context.HttpContext.Response.Output;
            correctOutput.Write("correct output");

            context.RouteData.Values["action"] = "action";
            context.RouteData.Values["controller"] = "controller";

            context.ActionDescriptor = new StaticActionDescriptor("controller", "action");

            var exceptionContext = new ExceptionContext();
            exceptionContext.HttpContext = context.HttpContext;

            //simulate one failure
            filter.OnActionExecuting(context);
            filter.OnException(exceptionContext);
            Assert.That(context.HttpContext.Response.Output, Is.SameAs(correctOutput));

            //simulate another failure
            filter.OnActionExecuting(context); //Pop one action onto the stack                       
            filter.OnException(exceptionContext);
            Assert.That(context.HttpContext.Response.Output, Is.SameAs(correctOutput));
        }
    }
}
