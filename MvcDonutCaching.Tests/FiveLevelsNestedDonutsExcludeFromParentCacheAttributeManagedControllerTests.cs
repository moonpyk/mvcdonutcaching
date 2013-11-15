using System;
using System.IO;
using System.Threading;
using DevTrends.MvcDonutCaching;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManagedControllerTests : FiveLevelsNestedDonutsControllerTests
    {
        protected override string ControllerUrl
        {
            get { return "/FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManaged"; }
        }

        [Test]//This is one of the tests I don't expect to be able to get working with the old code....
        public void EachLevelIsRenderedNoMoreOftenThanItsCachePolicyDuration()
        {
            DateTime runStartTime = DateTime.Now;
            var runUntil = runStartTime + TimeSpan.FromMilliseconds(2000);

            var failures = new StringWriter();
            var lastLevelTimes = RenderAndFetchLevelTimes();
            while (DateTime.Now < runUntil)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                var currentLevelTimes = RenderAndFetchLevelTimes();
                var runTimeSpan = DateTime.Now - runStartTime;

                Action<String, DateTime, DateTime, int> assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration =
                    (levelName, lastTime, currentTime, cachePolicyDurationMilliseconds) =>
                    {
                        const int toleranceMilliseconds = 50;
                        if (lastTime != currentTime)
                        {
                            var timeSinceLastRenderInMilliseconds = (int)(currentTime - lastTime).TotalMilliseconds;
                            var devianceInMilliseconds = cachePolicyDurationMilliseconds - timeSinceLastRenderInMilliseconds;
                            if (devianceInMilliseconds > toleranceMilliseconds)
                            {
                                failures.WriteLine("{0} re-rendered after {1} milliseconds. {2} milliseconds into the run. It should render no more often than every {3} +- {4} milliseconds",
                                    levelName, timeSinceLastRenderInMilliseconds, (int)runTimeSpan.TotalMilliseconds, cachePolicyDurationMilliseconds, toleranceMilliseconds);
                            }
                        }
                    };

                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level0", lastLevelTimes.Level0Duration5, currentLevelTimes.Level0Duration5, 500);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level1", lastLevelTimes.Level1Duration4, currentLevelTimes.Level1Duration4, 400);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level2", lastLevelTimes.Level2Duration3, currentLevelTimes.Level2Duration3, 300);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level3", lastLevelTimes.Level3Duration2, currentLevelTimes.Level3Duration2, 200);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level4", lastLevelTimes.Level4Duration1, currentLevelTimes.Level4Duration1, 100);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level5", lastLevelTimes.Level5Duration0, currentLevelTimes.Level5Duration0, 0);

                lastLevelTimes = currentLevelTimes;
            }

            if (failures.ToString() != string.Empty)
            {
                Assert.Fail(failures.ToString());
            }
        }
    }}