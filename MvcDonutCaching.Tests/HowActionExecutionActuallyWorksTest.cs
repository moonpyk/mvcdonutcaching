using System;
using System.Collections.Generic;
using System.IO;
using DevTrends.MvcDonutCaching;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class HowActionExecutionActuallyWorksTest
    {
        [Test]
        public void RendersInitialRequestCorrectlyForDepth1()
        {
            var actionContext = TestUtil.CreateMockActionExecutingControllerContext();

            var output = new StringWriter();
            actionContext.HttpContext.Response.Output = output;

            const string l1StartContent = "<L1>", l1EndContent = "</L1>";
            var expectedLevel1Result = string.Format("{0}{1}", l1StartContent, l1EndContent);

            using(actionContext.InvokeAction(action: "level1", output: expectedLevel1Result))
            {                
            }
            Assert.That(output.ToString(), Is.EqualTo(expectedLevel1Result));
        }

        [Test]
        public void RendersInitialRequestCorrectlyForDepth2()
        {
            var actionContext = TestUtil.CreateMockActionExecutingControllerContext();

            var output = new StringWriter();
            actionContext.HttpContext.Response.Output = output;

            const string l2Content = "<L2></L2>", l1Content = "<L1>{0}</L1>";

            var expectedLevel1Result = string.Format(l1Content, l2Content);

            using(actionContext.InvokeAction(action: "level1", output: l1Content))
            {
                using(actionContext.InvokeAction(action: "level2", output: l2Content))
                {
                    
                }   
            }            
            Assert.That(output.ToString(), Is.EqualTo(expectedLevel1Result));
        }

        [Test]
        public void RendersInitialRequestCorrectlyForDepth3()
        {
            var actionContext = TestUtil.CreateMockActionExecutingControllerContext();

            actionContext.ActionDescriptor = new StaticActionDescriptor(controllerName: "dummy", actionName: "level1");
            actionContext.ActionParameters = new Dictionary<string, object> { { "title", "level1" } };

            var output = new StringWriter();
            actionContext.HttpContext.Response.Output = output;

            const string l1Output = "<L1>{0}</L1>", l2Output = "<L2>{0}</L2>", l3Output = "<L3></L3>";

            var expectedLevel2Result = string.Format(l2Output, l3Output);
            var expectedLevel1Result = string.Format(l1Output, expectedLevel2Result);

            using (actionContext.InvokeAction(action: "L1", output: l1Output))
            {
                using (actionContext.InvokeAction(action: "L2", output: l2Output))
                {
                    using (actionContext.InvokeAction(action: "L3", output: l3Output))
                    {

                    }
                }
            }  

            Assert.That(output.ToString(), Is.EqualTo(expectedLevel1Result));

        }
    }
}