using System;
using System.IO;
using System.Threading;
using DevTrends.MvcDonutCaching;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManagedControllerTests : TestsBase
    {
        private string ControllerUrl
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

                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level0", lastLevelTimes.Level0, currentLevelTimes.Level0, 0);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level1", lastLevelTimes.Level1, currentLevelTimes.Level1, 100);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level2", lastLevelTimes.Level2, currentLevelTimes.Level2, 200);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level3", lastLevelTimes.Level3, currentLevelTimes.Level3, 300);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level4", lastLevelTimes.Level4, currentLevelTimes.Level4, 400);
                assertRenderTimeIsWithin50MillisecondsOfCachePolicyDuration("Level5", lastLevelTimes.Level5, currentLevelTimes.Level5, 500);

                lastLevelTimes = currentLevelTimes;
            }

            if (failures.ToString() != string.Empty)
            {
                Assert.Fail(failures.ToString());
            }
        }

        [SetUp]
        public void SetupTask()
        {
            EnableReplaceDonutsInChildActionsGlobally();
        }

        [Test]
        public void CanExecuteAtAll()
        {
            GetUrlContent(ControllerUrl);
        }

        [Test]
        public void EachLevelReturnsTheSameTimeOnFirstCall()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var levelTimes = new LevelRenderTimes(GetUrlContent(ControllerUrl));

                    AssertRenderedDuringSameRequest(
                        levelTimes.Level0,
                        levelTimes.Level1,
                        levelTimes.Level2,
                        levelTimes.Level3,
                        levelTimes.Level4,
                        levelTimes.Level5);
                });
        }


        [Test]
        public void OnlyLevel0HasUpdatedContentAfter10Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5;

                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    var levelTimes = FetchAndPrintLevelTimes();
                    AssertRenderedDuringSameRequest(originalRenderTime,
                        levelTimes.Level1,
                        levelTimes.Level2,
                        levelTimes.Level3,
                        levelTimes.Level4,
                        levelTimes.Level5);

                    AssertRenderedDuringLastRequest(levelTimes.Level0);
                });
        }

        [Test]
        public void OnlyLevel0_and1ShouldHaveCurrentValuesAfter110Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5;

                    Thread.Sleep(TimeSpan.FromMilliseconds(110));
                    var levelTimes = FetchAndPrintLevelTimes();
                    AssertRenderedDuringSameRequest(originalRenderTime,
                        levelTimes.Level2,
                        levelTimes.Level3,
                        levelTimes.Level4,
                        levelTimes.Level5);

                    AssertRenderedDuringLastRequest(levelTimes.Level0);
                    AssertRenderedDuringLastRequest(levelTimes.Level1);
                });
        }

        [Test]
        public void Level0_1_and2ShouldHaveCurrentValuesAfter210Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5;

                    Thread.Sleep(TimeSpan.FromMilliseconds(210));
                    var levelTimes = FetchAndPrintLevelTimes();

                    AssertRenderedDuringSameRequest(originalRenderTime, levelTimes.Level5, levelTimes.Level4, levelTimes.Level3);

                    AssertRenderedDuringLastRequest(levelTimes.Level0);
                    AssertRenderedDuringLastRequest(levelTimes.Level1);
                    AssertRenderedDuringLastRequest(levelTimes.Level2);
                });
        }

        [Test]
        public void Level0_1_2_and3ShouldHaveCurrentValuesAfter310Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5;

                    Thread.Sleep(TimeSpan.FromMilliseconds(310));
                    var levelTimes = FetchAndPrintLevelTimes();

                    AssertRenderedDuringSameRequest(originalRenderTime, levelTimes.Level5, levelTimes.Level4);

                    AssertRenderedDuringLastRequest(levelTimes.Level0);
                    AssertRenderedDuringLastRequest(levelTimes.Level1);
                    AssertRenderedDuringLastRequest(levelTimes.Level2);
                    AssertRenderedDuringLastRequest(levelTimes.Level3);
                });
        }


        [Test]
        public void AllButLevel5ShouldHaveCurrentValuesAfter410Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5;

                    Thread.Sleep(TimeSpan.FromMilliseconds(410));
                    var levelTimes = FetchAndPrintLevelTimes();
                    AssertRenderedDuringSameRequest(originalRenderTime, levelTimes.Level5);

                    AssertRenderedDuringLastRequest(levelTimes.Level0);
                    AssertRenderedDuringLastRequest(levelTimes.Level1);
                    AssertRenderedDuringLastRequest(levelTimes.Level2);
                    AssertRenderedDuringLastRequest(levelTimes.Level3);
                    AssertRenderedDuringLastRequest(levelTimes.Level4);                    
                });
        }

        [Test]
        public void AllLevelsHaveCurrentValuesAfter510Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5;

                    Thread.Sleep(TimeSpan.FromMilliseconds(510));
                    var levelTimes = FetchAndPrintLevelTimes();

                    AssertRenderedDuringLastRequest(levelTimes.Level0);
                    AssertRenderedDuringLastRequest(levelTimes.Level1);
                    AssertRenderedDuringLastRequest(levelTimes.Level2);
                    AssertRenderedDuringLastRequest(levelTimes.Level3);
                    AssertRenderedDuringLastRequest(levelTimes.Level4);
                    AssertRenderedDuringLastRequest(levelTimes.Level5);
                });
        }

        [Test]
        public void EachLevelIsRenderedNoLessOftenThanItsCachePolicyDuration()
        {
            var runStartTime = DateTime.Now;
            var runUntil = runStartTime + TimeSpan.FromMilliseconds(2000);

            var failures = new StringWriter();
            while (DateTime.Now < runUntil)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                var currentLevelTimes = RenderAndFetchLevelTimes();

                Action<String, DateTime, int> assertRenderTimeIsNotTooOld =
                    (levelName, currentTime, cachePolicyDurationMilliseconds) =>
                    {
                        const int toleranceMilliseconds = 50;
                        var millisecondsSinceLastRender = (int)(DateTime.Now - currentTime).TotalMilliseconds;
                        if ((millisecondsSinceLastRender - cachePolicyDurationMilliseconds) > toleranceMilliseconds)
                        {
                            failures.WriteLine("{0} was rendered {1} milliseconds ago even though policy is: {2}", levelName, millisecondsSinceLastRender, cachePolicyDurationMilliseconds);
                        }
                    };

                assertRenderTimeIsNotTooOld("Level0", currentLevelTimes.Level0, 0);
                assertRenderTimeIsNotTooOld("Level1", currentLevelTimes.Level1, 100);
                assertRenderTimeIsNotTooOld("Level2", currentLevelTimes.Level2, 200);
                assertRenderTimeIsNotTooOld("Level3", currentLevelTimes.Level3, 300);
                assertRenderTimeIsNotTooOld("Level4", currentLevelTimes.Level4, 400);
                assertRenderTimeIsNotTooOld("Level5", currentLevelTimes.Level5, 500);
            }

            if (failures.ToString() != string.Empty)
            {
                Assert.Fail(failures.ToString());
            }
        }

        private LevelRenderTimes FetchAndPrintLevelTimes()
        {
            var levelTimes = RenderAndFetchLevelTimes();
            PrintDurationsAndCurrentTime(levelTimes);
            return levelTimes;
        }

        protected LevelRenderTimes RenderAndFetchLevelTimes()
        {
            return new LevelRenderTimes(GetUrlContent(ControllerUrl));
        }

        private void PrintDurationsAndCurrentTime(LevelRenderTimes levelTimes)
        {
            var now = DateTime.Now;

            Console.WriteLine("Time is:            {0}", now.ToString("o"));
            Console.WriteLine();

            Console.WriteLine("Level0 is: {0}", levelTimes.Level0.ToString("o"));
            Console.WriteLine("Level1 is: {0}", levelTimes.Level1.ToString("o"));
            Console.WriteLine("Level2 is: {0}", levelTimes.Level2.ToString("o"));
            Console.WriteLine("Level3 is: {0}", levelTimes.Level3.ToString("o"));
            Console.WriteLine("Level4 is: {0}", levelTimes.Level4.ToString("o"));
            Console.WriteLine("Level5 is: {0}", levelTimes.Level5.ToString("o"));
            Console.WriteLine();

            Console.WriteLine("Level0Duration5Age is: {0}", (int)(now - levelTimes.Level0).TotalMilliseconds);
            Console.WriteLine("Level1Duration4Age is: {0}", (int)(now - levelTimes.Level1).TotalMilliseconds);
            Console.WriteLine("Level2Duration3Age is: {0}", (int)(now - levelTimes.Level2).TotalMilliseconds);
            Console.WriteLine("Level3Duration2Age is: {0}", (int)(now - levelTimes.Level3).TotalMilliseconds);
            Console.WriteLine("Level4Duration1Age is: {0}", (int)(now - levelTimes.Level4).TotalMilliseconds);
            Console.WriteLine("Level5Duration0Age is: {0}", (int)(now - levelTimes.Level5).TotalMilliseconds);
            Console.WriteLine();
        }

        protected class LevelRenderTimes
        {
            public readonly DateTime Level0;
            public readonly DateTime Level1;
            public readonly DateTime Level2;
            public readonly DateTime Level3;
            public readonly DateTime Level4;
            public readonly DateTime Level5;

            public LevelRenderTimes(string viewOutPut)
            {
                var levels = viewOutPut.Replace("<br/>", "").Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                Level0 = DateTime.Parse(levels[0].Split('#')[1]);
                Level1 = DateTime.Parse(levels[1].Split('#')[1]);
                Level2 = DateTime.Parse(levels[2].Split('#')[1]);
                Level3 = DateTime.Parse(levels[3].Split('#')[1]);
                Level4 = DateTime.Parse(levels[4].Split('#')[1]);
                Level5 = DateTime.Parse(levels[5].Split('#')[1]);
            }
        }
    }}