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
        public void RenderesCorrectOutputWhenEverythingIsCached()
        {
            var scenario = ThreeLevelNestedOutputActionScenario.CreateByExecutingUnCached();
            scenario.ExecuteAndValidateOutputFirstAsIsAndThenFullyCached();
        }

        [Test]
        public void RendersCorrectOutputWhenLevel1IsNotCached()
        {
            var scenario = ThreeLevelNestedOutputActionScenario.CreateByExecutingUnCached();
            scenario.EvictLevel1FromCache();
            scenario.Level1StartOutput = Guid.NewGuid().ToString();
            scenario.Level1EndOutput = Guid.NewGuid().ToString();
            var output = scenario.ExecuteAndValidateOutputFirstAsIsAndThenFullyCached();
            Assert.That(output, Contains.Substring(scenario.Level1StartOutput));
            Assert.That(output, Contains.Substring(scenario.Level1EndOutput));
        }

        [Test]
        public void RendersCorrectOutputWhenLevel2IsNotCached()
        {
            var scenario = ThreeLevelNestedOutputActionScenario.CreateByExecutingUnCached();
            scenario.EvictLevel2FromCache();
            scenario.Level2StartOutput = Guid.NewGuid().ToString();
            scenario.Level2EndOutput = Guid.NewGuid().ToString();
            var output = scenario.ExecuteAndValidateOutputFirstAsIsAndThenFullyCached();
            Assert.That(output, Contains.Substring(scenario.Level2StartOutput));
            Assert.That(output, Contains.Substring(scenario.Level2EndOutput));
        }

        [Test]
        public void RendersCorrectOutputWhenLevel3IsNotCached()
        {
            var scenario = ThreeLevelNestedOutputActionScenario.CreateByExecutingUnCached();
            scenario.EvictLevel3FromCache();
            scenario.Level3Output = Guid.NewGuid().ToString();
            var output = scenario.ExecuteAndValidateOutputFirstAsIsAndThenFullyCached();
            Assert.That(output, Contains.Substring(scenario.Level3Output));
        }

        [Test]
        public void RendersCorrectOutputWhenLevel2And3AreNotCached()
        {
            var scenario = ThreeLevelNestedOutputActionScenario.CreateByExecutingUnCached();
            scenario.EvictLevel2FromCache();
            scenario.EvictLevel3FromCache();
            scenario.Level2StartOutput = Guid.NewGuid().ToString();
            scenario.Level2EndOutput = Guid.NewGuid().ToString();
            scenario.Level3Output = Guid.NewGuid().ToString();
            var output = scenario.ExecuteAndValidateOutputFirstAsIsAndThenFullyCached();
            Assert.That(output, Contains.Substring(scenario.Level2StartOutput));
            Assert.That(output, Contains.Substring(scenario.Level2EndOutput));
            Assert.That(output, Contains.Substring(scenario.Level3Output));
        }

        [Test]
        public void RendersCorrectOutputWhenLevel1And3AreNotCached()
        {
            var scenario = ThreeLevelNestedOutputActionScenario.CreateByExecutingUnCached();
            scenario.EvictLevel1FromCache();
            scenario.EvictLevel3FromCache();
            scenario.Level1StartOutput = Guid.NewGuid().ToString();
            scenario.Level1EndOutput = Guid.NewGuid().ToString();
            scenario.Level3Output = Guid.NewGuid().ToString();
            var output = scenario.ExecuteAndValidateOutputFirstAsIsAndThenFullyCached();
            Assert.That(output, Contains.Substring(scenario.Level1StartOutput));
            Assert.That(output, Contains.Substring(scenario.Level1EndOutput));
            Assert.That(output, Contains.Substring(scenario.Level3Output));
        }

        [Test]
        public void RendersCorrectOutputWhenLevel1And2AreNotCached()
        {
            var scenario = ThreeLevelNestedOutputActionScenario.CreateByExecutingUnCached();
            scenario.EvictLevel1FromCache();
            scenario.EvictLevel2FromCache();
            scenario.Level1StartOutput = Guid.NewGuid().ToString();
            scenario.Level1EndOutput = Guid.NewGuid().ToString();
            scenario.Level2StartOutput = Guid.NewGuid().ToString();
            scenario.Level2EndOutput = Guid.NewGuid().ToString();
            var output = scenario.ExecuteAndValidateOutputFirstAsIsAndThenFullyCached();
            Assert.That(output, Contains.Substring(scenario.Level1StartOutput));
            Assert.That(output, Contains.Substring(scenario.Level1EndOutput));
            Assert.That(output, Contains.Substring(scenario.Level2StartOutput));
            Assert.That(output, Contains.Substring(scenario.Level2EndOutput));
        }

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
            DonutOutputManager.ResultExecuted(context);
            Assert.That(context.HttpContext.Response.Output, Is.SameAs(rootOutput), "output should have been restored");
        }

        [Test]
        public void SetsDonutOutputListAndDonutListCorrectly()
        {
            var context = TestUtil.CreateMockActionExecutingControllerContext();

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
            DonutOutputManager.ActionExecuting(context); //Level1
            Assert.That(context.HttpContext.Response.Output, Is.Not.SameAs(rootOutput), "output should have been replaced");
            context.HttpContext.Response.Output.Write(level1StartOutput);


            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level2");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level2_1" } };
            DonutOutputManager.ActionExecuting(context); //Level2
            context.HttpContext.Response.Output.Write(level2StartOutput);

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level3");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level3" } };
            DonutOutputManager.ActionExecuting(context); //Level3
            context.HttpContext.Response.Output.Write(level3Output);
            level3Donut1 = DonutOutputManager.ResultExecuted(context); //Level2
            context.HttpContext.Response.Output.Write(level2EndOutput);
            level2Donut1 = DonutOutputManager.ResultExecuted(context); //Level1

            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level2");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level2_1" } };
            DonutOutputManager.ActionExecuting(context); //Level2
            context.HttpContext.Response.Output.Write(level2StartOutput);


            context.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level3");
            context.ActionParameters = new Dictionary<string, object>() { { "title", "level3_2" } };
            DonutOutputManager.ActionExecuting(context); //Level3
            context.HttpContext.Response.Output.Write(level3Output);
            level3Donut2 = DonutOutputManager.ResultExecuted(context); //Level2
            context.HttpContext.Response.Output.Write(level2EndOutput);
            level2Donut2 = DonutOutputManager.ResultExecuted(context); //Level1

            context.HttpContext.Response.Output.Write(level1EndOutput);
            level1Donut = DonutOutputManager.ResultExecuted(context); //Done

            Assert.That(level3Donut1.OutputSegments, Is.EqualTo(new[] {level3Output}));
            Assert.That(level3Donut1.Children.Count, Is.EqualTo(0));

            Assert.That(level3Donut2.OutputSegments, Is.EqualTo(new[] {level3Output}));
            Assert.That(level3Donut2.Children.Count, Is.EqualTo(0));

            Assert.That(level2Donut1.OutputSegments, Is.EqualTo(new[] {level2StartOutput, level2EndOutput}));
            Assert.That(level2Donut1.Children.Count, Is.EqualTo(1));

            Assert.That(level2Donut2.OutputSegments, Is.EqualTo(new[] {level2StartOutput, level2EndOutput}));
            Assert.That(level2Donut2.Children.Count, Is.EqualTo(1));

            Assert.That(level1Donut.OutputSegments, Is.EqualTo(new[] {level1StartOutput, "", level1EndOutput}));
            Assert.That(level1Donut.Children.Count, Is.EqualTo(2));
        }       

        public class ThreeLevelNestedOutputActionScenario
        {
            private Donut _level1Donut;
            private Donut _level2Donut;
            private Donut _level3Donut;


            private StaticActionDescriptor Level1DonutActionDescriptor{ get { return (StaticActionDescriptor)_level1Donut.ControllerAction.ActionDescriptor; }}
            private StaticActionDescriptor Level2DonutActionDescriptor { get { return (StaticActionDescriptor)_level2Donut.ControllerAction.ActionDescriptor; } }
            private StaticActionDescriptor Level3DonutActionDescriptor { get { return (StaticActionDescriptor)_level3Donut.ControllerAction.ActionDescriptor; } }
           
            // ReSharper disable MemberCanBePrivate.Global
            public string Level1StartOutput = "_1";
            public string Level1EndOutput = "1_";
            public string Level2StartOutput = "_2";
            public string Level2EndOutput = "2_";
                public string Level3Output = "_3_";
            // ReSharper restore MemberCanBePrivate.Global

            public void EvictLevel1FromCache()
            {
                _level1Donut = null;
            }

            public void EvictLevel2FromCache()
            {
                _level2Donut = null;
            }

            public void EvictLevel3FromCache()
            {
                _level3Donut = null;
            }

            public string CorrectUnCachedOutputBasedOnOutputProperties
            {
                get { return string.Format(@"{0}{1}{2}{3}{4}", Level1StartOutput, Level2StartOutput, Level3Output, Level2EndOutput, Level1EndOutput); }
            }

            public Action<ActionExecutingContext> ExecuteLevel3;
            public Action<ActionExecutingContext> ExecuteLevel2;
            public Action<ActionExecutingContext> ExecuteLevel1;

            public static ThreeLevelNestedOutputActionScenario CreateByExecutingUnCached()
            {
                var scenario = new ThreeLevelNestedOutputActionScenario();
                scenario.ExecuteScenario();
                return scenario;
            }

            private ThreeLevelNestedOutputActionScenario()
            {
                ExecuteLevel3 = actionContext =>
                {
                    if(_level3Donut != null)
                    {
                        _level3Donut.Execute(actionContext);
                        return;
                    }
                    
                    actionContext.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level3");
                    actionContext.ActionParameters = new Dictionary<string, object>() {{"title", "level3"}};
                    DonutOutputManager.ActionExecuting(actionContext); //Level3
                    actionContext.HttpContext.Response.Output.Write(Level3Output);

                    _level3Donut = DonutOutputManager.ResultExecuted(actionContext);
                };

                ExecuteLevel2 = actionContext =>
                {
                    if(_level2Donut != null)
                    {
                        _level2Donut.Execute(actionContext);
                        return;
                    }
                    
                    actionContext.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level2");
                    actionContext.ActionParameters = new Dictionary<string, object>() {{"title", "level2_1"}};
                    DonutOutputManager.ActionExecuting(actionContext);
                    actionContext.HttpContext.Response.Output.Write(Level2StartOutput);

                    ExecuteLevel3(actionContext);

                    actionContext.HttpContext.Response.Output.Write(Level2EndOutput);

                    _level2Donut = DonutOutputManager.ResultExecuted(actionContext);
                };

                ExecuteLevel1 = actionContext =>
                {
                    if(_level1Donut != null)
                    {
                        _level1Donut.Execute(actionContext);
                        return;
                    }
                    
                    actionContext.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level1");
                    actionContext.ActionParameters = new Dictionary<string, object>() {{"title", "level1"}};
                    DonutOutputManager.ActionExecuting(actionContext); //Level1
                    actionContext.HttpContext.Response.Output.Write(Level1StartOutput);

                    ExecuteLevel2(actionContext);

                    actionContext.HttpContext.Response.Output.Write(Level1EndOutput);
                    _level1Donut = DonutOutputManager.ResultExecuted(actionContext);
                };   
            }

            private string ExecuteScenario()
            {               
                var context = TestUtil.CreateMockActionExecutingControllerContext();

                var rootOutput = new StringWriter();
                context.HttpContext.Response.Output = rootOutput;
                ExecuteLevel1(context);

                Level1DonutActionDescriptor.ExcecuteDelegate = ExecuteLevel1;
                Level2DonutActionDescriptor.ExcecuteDelegate = ExecuteLevel2;
                Level3DonutActionDescriptor.ExcecuteDelegate = ExecuteLevel3;

                return rootOutput.ToString();
            }

            public string ExecuteAndValidateOutputFirstAsIsAndThenFullyCached(string expectedOutput = null, params object[] formatValues)
            {
                if(expectedOutput == null)
                {
                    expectedOutput = CorrectUnCachedOutputBasedOnOutputProperties;
                }
                Assert.That(ExecuteScenario(), Is.EqualTo(string.Format(expectedOutput, formatValues)));

                var output = ExecuteScenario();
                Assert.That(output, Is.EqualTo(string.Format(expectedOutput, formatValues)));
                return output;
            }
        }
    }

    public class StaticActionDescriptor : ActionDescriptor
    {
        private readonly string _actionName;
        public Action<ActionExecutingContext> ExcecuteDelegate;
        public Donut DonutToDelegateTo;
        private ControllerDescriptor _controllerDescriptor;

        public StaticActionDescriptor(string controllerName, string actionName, Action<ControllerContext> excecuteDelegate = null, Donut executeDonutDelegate = null)
        {
            _actionName = actionName;
            DonutToDelegateTo = executeDonutDelegate;
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

            if (DonutToDelegateTo != null)
            {
                DonutToDelegateTo.Execute((ActionExecutingContext)controllerContext);
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
