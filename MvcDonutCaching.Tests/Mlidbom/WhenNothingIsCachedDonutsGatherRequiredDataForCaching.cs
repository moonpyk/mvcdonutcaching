﻿using System;
using System.Linq;
using DevTrends.MvcDonutCaching.Mlidbom;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture]
    public class WhenNothingIsCachedDonutsGatherRequiredDataForCaching
    {
        [Test]
        public void GathersCorrectDataForInitialRequestForDepth1()
        {
            var actionContext = TestUtil.CreateMockActionExecutingControllerContext();

            const string l2Output = "<L1></L1>";

            IDonut donut = null;
            using(actionContext.InvokeAction(output: l2Output, afterResultExecuted: d => donut = d)) {}

            Assert.That(donut.SortedChildren.Count(), Is.EqualTo(0));
            Assert.That(donut.SortedOutputSegments.Count(), Is.EqualTo(1));
            Assert.That(donut.SortedOutputSegments.ElementAt(0), Is.EqualTo(l2Output));
        }

        [Test]
        public void GathersCorrectDataForInitialRequestForDepth2()
        {
            var actionContext = TestUtil.CreateMockActionExecutingControllerContext();

            const string l2Content = "<L2></L2>", l1Template = "<L1>{0}</L1>";

            IDonut level1Donut = null, level2Donut = null;

            using (actionContext.InvokeAction(output: l1Template, afterResultExecuted: donut => level1Donut = donut))
            {
                using (actionContext.InvokeAction(output: l2Content, afterResultExecuted: donut => level2Donut = donut)) { }
            }

            Assert.That(level2Donut.SortedChildren.Count(), Is.EqualTo(0));
            Assert.That(level2Donut.SortedOutputSegments.Count(), Is.EqualTo(1));
            Assert.That(level2Donut.SortedOutputSegments.ElementAt(0), Is.EqualTo(l2Content));

            Assert.That(level1Donut.SortedChildren.Count(), Is.EqualTo(1));
            Assert.That(level1Donut.SortedOutputSegments.Count(), Is.EqualTo(2));
            Assert.That(level1Donut.SortedOutputSegments.ElementAt(0), Is.EqualTo("<L1>"));
            Assert.That(level1Donut.SortedOutputSegments.ElementAt(1),Is.EqualTo("</L1>"));
        }

        [Test]
        public void GathersCorrectDataForInitialRequestForDepth3()
        {
            var actionContext = TestUtil.CreateMockActionExecutingControllerContext();

            const string l1Template = "<L1>{0}</L1>", l2Template = "<L2>{0}</L2>", l3Output = "<L3></L3>";

            IDonut level1Donut = null, level2Donut = null, level3Donut = null;

            using (actionContext.InvokeAction(output: l1Template, afterResultExecuted: donut => level1Donut = donut))
            {
                using(actionContext.InvokeAction(output: l2Template, afterResultExecuted: donut => level2Donut = donut))
                {
                    using (actionContext.InvokeAction(output: l3Output, afterResultExecuted: donut => level3Donut = donut)) { }
                }
            }

            Assert.That(level3Donut.SortedChildren.Count(), Is.EqualTo(0));
            Assert.That(level3Donut.SortedOutputSegments.Count(), Is.EqualTo(1));
            Assert.That(level3Donut.SortedOutputSegments.ElementAt(0), Is.EqualTo(l3Output));

            Assert.That(level2Donut.SortedChildren.Count(), Is.EqualTo(1));
            Assert.That(level2Donut.SortedOutputSegments.Count(), Is.EqualTo(2));
            Assert.That(level2Donut.SortedOutputSegments.ElementAt(0), Is.EqualTo("<L2>"));
            Assert.That(level2Donut.SortedOutputSegments.ElementAt(1), Is.EqualTo("</L2>"));

            Assert.That(level1Donut.SortedChildren.Count(), Is.EqualTo(1));
            Assert.That(level1Donut.SortedOutputSegments.Count(), Is.EqualTo(2));
            Assert.That(level1Donut.SortedOutputSegments.ElementAt(0), Is.EqualTo("<L1>"));
            Assert.That(level1Donut.SortedOutputSegments.ElementAt(1), Is.EqualTo("</L1>"));
        }

        [Test]
        public void GathersCorrectDataForInitialRequestForDepth3WithDualChildrenForEachChildLevelAndAdjacentChildren()
        {
            var actionContext = TestUtil.CreateMockActionExecutingControllerContext();

            const string l1Template = "<L1>{0}{1}</L1>", l2Template = "<L2>{0}Between{1}</L2>", l3Output = "<L3></L3>";

            IDonut level1Donut = null, level2Donut1 = null, level2Donut2 = null, level3Donut1 = null, level3Donut2 = null, level3Donut3 = null, level3Donut4 = null;

            using (actionContext.InvokeAction(output: l1Template, afterResultExecuted: donut => level1Donut = donut))
            {
                using (actionContext.InvokeAction(output: l2Template, afterResultExecuted: donut => level2Donut1 = donut))
                {
                    using (actionContext.InvokeAction(output: l3Output, afterResultExecuted: donut => level3Donut1 = donut)) { }
                    using (actionContext.InvokeAction(output: l3Output, afterResultExecuted: donut => level3Donut2 = donut)) { }
                }
                using (actionContext.InvokeAction(output: l2Template, afterResultExecuted: donut => level2Donut2 = donut))
                {
                    using (actionContext.InvokeAction(output: l3Output, afterResultExecuted: donut => level3Donut3 = donut)) { }
                    using (actionContext.InvokeAction(output: l3Output, afterResultExecuted: donut => level3Donut4 = donut)) { }
                }
            }

            Assert.That(level3Donut1.SortedChildren.Count(), Is.EqualTo(0));
            Assert.That(level3Donut1.SortedOutputSegments.Count(), Is.EqualTo(1));
            Assert.That(level3Donut1.SortedOutputSegments.ElementAt(0), Is.EqualTo(l3Output));

            Assert.That(level2Donut1.SortedChildren.Count(), Is.EqualTo(2));
            Assert.That(level2Donut1.SortedOutputSegments.Count(), Is.EqualTo(3));
            Assert.That(level2Donut1.SortedOutputSegments.ElementAt(0), Is.EqualTo("<L2>"));
            Assert.That(level2Donut1.SortedOutputSegments.ElementAt(1), Is.EqualTo("Between"));
            Assert.That(level2Donut1.SortedOutputSegments.ElementAt(2), Is.EqualTo("</L2>"));

            Assert.That(level1Donut.SortedChildren.Count(), Is.EqualTo(2));
            Assert.That(level1Donut.SortedOutputSegments.Count(), Is.EqualTo(3));
            Assert.That(level1Donut.SortedOutputSegments.ElementAt(0), Is.EqualTo("<L1>"));
            Assert.That(level1Donut.SortedOutputSegments.ElementAt(1), Is.EqualTo(""));
            Assert.That(level1Donut.SortedOutputSegments.ElementAt(2), Is.EqualTo("</L1>"));
        }
    }
}
