using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture]
    public class DonutOutputManagerTests
    {
        [Test]
        public void ReplacesAndRestoresResponseOutPutCorrectly()
        {
            var context = TestUtil.CreateMockActionExecutingControllerContext();

            var rootOutput = new StringWriter();
            context.HttpContext.Response.Output = rootOutput;

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level1");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level1" } };
            DonutOutputManager.ActionExecuting(context);
            Assert.That(context.HttpContext.Response.Output, Is.Not.SameAs(rootOutput), "output should have been replaced");
            DonutOutputManager.ResultExecutionSucceeded(context);
            Assert.That(context.HttpContext.Response.Output, Is.SameAs(rootOutput), "output should have been restored");
        }

    }

    public class StaticActionDescriptor : ActionDescriptor
    {
        private readonly string _actionName;
        public Action<ActionExecutingContext> ExcecuteDelegate;
        private ControllerDescriptor _controllerDescriptor;

        public StaticActionDescriptor(string controllerName, string actionName, Action<ControllerContext> excecuteDelegate = null)
        {
            _actionName = actionName;
            ExcecuteDelegate = excecuteDelegate;
            _controllerDescriptor = new StaticControllerDescriptor(controllerName);
        }

        override public object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            if (ExcecuteDelegate != null)
            {
                ExcecuteDelegate((ActionExecutingContext)controllerContext);
                return null;
            }        

            throw new NotImplementedException();
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
