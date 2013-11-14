using System.Collections;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    internal static class TestUtil
    {
        internal static ActionExecutingContext CreateMockActionExecutingControllerContext()
        {
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            response.SetupProperty(resp => resp.Output);
            response.Setup(resp => resp.Write(It.IsAny<string>())).Callback((string output) => response.Object.Output.Write(output));

            var httpContextMock = new Mock<HttpContextBase>(MockBehavior.Strict);
            httpContextMock.Setup(http => http.Response).Returns(response.Object);
            IDictionary items = new Hashtable();
            httpContextMock.Setup(http => http.Items).Returns(items);

            var context = new ActionExecutingContext();
            context.HttpContext = httpContextMock.Object;

            context.HttpContext.Response.Output = new StringWriter();

            return context;
        }

        public static void AssertOutputEquals(this ControllerContext me, string expected)
        {
            Assert.That(me.HttpContext.Response.Output.ToString(), Is.EqualTo(expected));
        }
    }
}