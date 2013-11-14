using System.Collections;
using System.Web;
using System.Web.Mvc;
using Moq;

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


        return context;
    }
}