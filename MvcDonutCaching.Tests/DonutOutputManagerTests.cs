using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Moq;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class DonutOutputManagerTests
    {
        [Test]
        public void ReplacesAndRestoresResponseOutPutCorrectly()
        {
            var context = CreateMockActionExecutingControllerContext().Object;

            var rootOutput = new StringWriter();
            context.HttpContext.Response.Output = rootOutput;

            context.ActionDescriptor = new StaticActionDescriptor(controllerName:"dummy", actionName:"level1");
            context.ActionParameters = new Dictionary<string, object>(){ {"title", "level1"} };
            DonutOutputManager.Push(context);
            Assert.That(context.HttpContext.Response.Output, Is.Not.SameAs(rootOutput), "output should have been replaced");
            context.HttpContext.Response.Output.WriteLine("");
            context.HttpContext.Response.Output.WriteLine("1");

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level2");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level2" } };
            DonutOutputManager.Push(context);
            context.HttpContext.Response.Output.WriteLine(" 2");

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level3");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level3" } };
            DonutOutputManager.Push(context);
            context.HttpContext.Response.Output.WriteLine("  3");
            DonutOutputManager.Pop(context);
            context.HttpContext.Response.Output.WriteLine(" 2");
            DonutOutputManager.Pop(context);
            context.HttpContext.Response.Output.WriteLine("1");
            DonutOutputManager.Pop(context);
            Assert.That(context.HttpContext.Response.Output, Is.SameAs(rootOutput), "output should have been restored");
            Assert.That(rootOutput.ToString(),
                Is.EqualTo(
                    @"
1
 2
  3
 2
1
"));
        }

        [Test]
        public void SetsDonutOutputListAndDonutListCorrectly()
        {
            var context = CreateMockActionExecutingControllerContext().Object;

            var rootOutput = new StringWriter();
            context.HttpContext.Response.Output = rootOutput;

            Donut level1Donut, level2Donut1, level2Donut2, level3Donut1, level3Donut2 = null;
            const string level1StartOutput = "_1",
                level1EndOutput = "1_",
                level2StartOutput = "_2",
                level2EndOutput = "2_",
                level3Output = "_3_";

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level1");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level1" } };
            DonutOutputManager.Push(context); //Level1
            Assert.That(context.HttpContext.Response.Output, Is.Not.SameAs(rootOutput), "output should have been replaced");
            context.HttpContext.Response.Output.Write(level1StartOutput);


            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level2");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level2_1" } };
            DonutOutputManager.Push(context); //Level2
            context.HttpContext.Response.Output.Write(level2StartOutput);

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level3");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level3" } };
            DonutOutputManager.Push(context); //Level3
            context.HttpContext.Response.Output.Write(level3Output);
            level3Donut1 = DonutOutputManager.Pop(context); //Level2
            context.HttpContext.Response.Output.Write(level2EndOutput);
            level2Donut1 = DonutOutputManager.Pop(context); //Level1

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level2");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level2_1" } };
            DonutOutputManager.Push(context); //Level2
            context.HttpContext.Response.Output.Write(level2StartOutput);


            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level3");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level3_2" } };
            DonutOutputManager.Push(context); //Level3
            context.HttpContext.Response.Output.Write(level3Output);
            level3Donut2 = DonutOutputManager.Pop(context); //Level2
            context.HttpContext.Response.Output.Write(level2EndOutput);
            level2Donut2 = DonutOutputManager.Pop(context); //Level1

            context.HttpContext.Response.Output.Write(level1EndOutput);
            level1Donut = DonutOutputManager.Pop(context); //Done

            Assert.That(level3Donut1.OutputList, Is.EqualTo(new[] {level3Output}));
            Assert.That(level3Donut1.ChildActions.Count, Is.EqualTo(0));

            Assert.That(level3Donut2.OutputList, Is.EqualTo(new[] {level3Output}));
            Assert.That(level3Donut2.ChildActions.Count, Is.EqualTo(0));

            Assert.That(level2Donut1.OutputList, Is.EqualTo(new[] {level2StartOutput, level2EndOutput}));
            Assert.That(level2Donut1.ChildActions.Count, Is.EqualTo(1));

            Assert.That(level2Donut2.OutputList, Is.EqualTo(new[] {level2StartOutput, level2EndOutput}));
            Assert.That(level2Donut2.ChildActions.Count, Is.EqualTo(1));

            Assert.That(level1Donut.OutputList, Is.EqualTo(new[] {level1StartOutput, "", level1EndOutput}));
            Assert.That(level1Donut.ChildActions.Count, Is.EqualTo(2));
        }

        [Test]
        public void henEveryThingIsCached()
        {
            var context = CreateMockActionExecutingControllerContext().Object;

            var rootOutput = new StringWriter();
            context.HttpContext.Response.Output = rootOutput;

            Donut level1Donut, level2Donut1, level2Donut2, level3Donut1, level3Donut2 = null;
            const string level1StartOutput = "_1",
                level1EndOutput = "1_",
                level2StartOutput = "_2",
                level2EndOutput = "2_",
                level3Output = "_3_";


        }

        private static Mock<ActionExecutingContext> CreateMockActionExecutingControllerContext()
        {
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            response.SetupProperty(resp => resp.Output);

            var httpContextMock = new Mock<HttpContextBase>(MockBehavior.Strict);
            httpContextMock.Setup(http => http.Response).Returns(response.Object);
            IDictionary items = new Hashtable();
            httpContextMock.Setup(http => http.Items).Returns(items);

            var contextMock = new Mock<ActionExecutingContext>(MockBehavior.Strict);
            contextMock.Setup(context => context.HttpContext).Returns(httpContextMock.Object);
            contextMock.SetupProperty(context => context.ActionDescriptor);
            contextMock.SetupProperty(context => context.ActionParameters);

            return contextMock;
        }
    }

    public class StaticActionDescriptor : ActionDescriptor
    {
        private readonly string _controllerName;
        private readonly string _actionName;
        private ControllerDescriptor _controllerDescriptor;

        public StaticActionDescriptor(string controllerName, string actionName)
        {
            _controllerName = controllerName;
            _actionName = actionName;
            _controllerDescriptor = new StaticControllerDescriptor(_controllerName);
        }

        override public object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }

        override public ParameterDescriptor[] GetParameters()
        {
            throw new System.NotImplementedException();
        }

        override public string ActionName { get { return  _actionName; } }

        override public ControllerDescriptor ControllerDescriptor { get { return _controllerDescriptor; } }
    }

    public class StaticControllerDescriptor : ControllerDescriptor
    {
        private readonly string _controllerName;

        public StaticControllerDescriptor(string controllerName)
        {
            _controllerName = controllerName;
        }

        override public ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            throw new NotImplementedException();
        }

        override public string ControllerName { get { return _controllerName; } }

        override public ActionDescriptor[] GetCanonicalActions()
        {
            throw new NotImplementedException();
        }

        override public Type ControllerType { get { throw new NotImplementedException(); } }
    }
}
