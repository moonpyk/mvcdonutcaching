﻿using System;
using System.IO;
using System.Threading;
using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture, ExclusivelyUses(Controller)]
    public class FiveLevelsNestedDonutsLevel2And4HaveNoAttributeTests : ControllerTestBase
    {
        private const string Controller = "FiveLevelsNestedDonutsLevel2And4HaveNoAttribute";
        override protected string ControllerName { get { return Controller; } }

        [SetUp]
        public void SetupTask()
        {
            EnableReplaceDonutsInChildActionsGlobally();
        }

        [Test]
        public void CanExecuteAtAll()
        {
            ExecuteDefaultAction();
        }

        [Test]
        public void EachLevelReturnsTheSameTimeOnFirstCall()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var levelTimes = new LevelRenderTimes(ExecuteDefaultAction());

                    AssertRenderedDuringSameRequest(
                        levelTimes.Level0Duration5,
                        levelTimes.Level1Duration4,
                        levelTimes.Level2NoAttribute,
                        levelTimes.Level3Duration2,
                        levelTimes.Level4NoAttribute,
                        levelTimes.Level5Duration0);
                });
        }


        [Test]//Todo:This newer test variant probably makes most of the other tests in this class redundant. Remove them if test run time gets too long.
        public void EachLevelIsRenderedNoLessOftenThanItsCachePolicyDuration_ChildActionsWithNoAttributeAreCachedWithTheirParentsPolicy()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var runStartTime = DateTime.Now;
                    var runUntil = runStartTime + TimeSpan.FromMilliseconds(2000);

                    var failures = new StringWriter();
                    while(DateTime.Now < runUntil)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(10));
                        var currentLevelTimes = RenderAndFetchLevelTimes();

                        Action<String, DateTime, int> assertRenderTimeIsNotTooOld =
                            (levelName, currentTime, cachePolicyDurationMilliseconds) =>
                            {
                                const int toleranceMilliseconds = 50;
                                var millisecondsSinceLastRender = (int)(DateTime.Now - currentTime).TotalMilliseconds;
                                if((millisecondsSinceLastRender - cachePolicyDurationMilliseconds) > toleranceMilliseconds)
                                {
                                    failures.WriteLine("{0} was rendered {1} milliseconds ago even though policy is: {2}",
                                        levelName,
                                        millisecondsSinceLastRender,
                                        cachePolicyDurationMilliseconds);
                                }
                            };

                        assertRenderTimeIsNotTooOld("Level0", currentLevelTimes.Level0Duration5, 500);
                        assertRenderTimeIsNotTooOld("Level1", currentLevelTimes.Level1Duration4, 400);
                        assertRenderTimeIsNotTooOld("Level2", currentLevelTimes.Level2NoAttribute, 400); //No attribute means it is cached with its parent.
                        assertRenderTimeIsNotTooOld("Level3", currentLevelTimes.Level3Duration2, 200);
                        assertRenderTimeIsNotTooOld("Level4", currentLevelTimes.Level4NoAttribute, 200); //No attributte means it is cached with its parent.
                        assertRenderTimeIsNotTooOld("Level5", currentLevelTimes.Level5Duration0, 0);
                    }

                    if(failures.ToString() != string.Empty)
                    {
                        Assert.Fail(failures.ToString());
                    }
                });
        }


        [Test]
        public void OnlyLevel5HasUpdatedContentAfter10Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5Duration0;

                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    var levelTimes = FetchAndPrintLevelTimes();
                    AssertRenderedDuringSameRequest(originalRenderTime,
                        levelTimes.Level0Duration5,
                        levelTimes.Level1Duration4,
                        levelTimes.Level2NoAttribute,
                        levelTimes.Level3Duration2,
                        levelTimes.Level4NoAttribute);

                    AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
                });
        }

        [Test]
        public void Level5_4_and_3ShouldHaveCurrentValuesAfter210Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5Duration0;

                    Thread.Sleep(TimeSpan.FromMilliseconds(210));
                    var levelTimes = FetchAndPrintLevelTimes();

                    AssertRenderedDuringSameRequest(originalRenderTime, levelTimes.Level0Duration5, levelTimes.Level1Duration4, levelTimes.Level2NoAttribute);

                    AssertRenderedDuringLastRequest(levelTimes.Level3Duration2);
                    AssertRenderedDuringLastRequest(levelTimes.Level4NoAttribute);
                    AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
                });
        }

        [Test]
        public void AllButRootShouldHaveCurrentValuesAfter410Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5Duration0;

                    Thread.Sleep(TimeSpan.FromMilliseconds(410));
                    var levelTimes = FetchAndPrintLevelTimes();
                    AssertRenderedDuringSameRequest(originalRenderTime, levelTimes.Level0Duration5);

                    AssertRenderedDuringLastRequest(levelTimes.Level1Duration4);
                    AssertRenderedDuringLastRequest(levelTimes.Level2NoAttribute);
                    AssertRenderedDuringLastRequest(levelTimes.Level3Duration2);
                    AssertRenderedDuringLastRequest(levelTimes.Level4NoAttribute);
                    AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
                });
        }

        [Test]
        public void AllLevelsHaveCurrentValuesAfter510Milliseconds()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var originalRenderTime = RenderAndFetchLevelTimes().Level5Duration0;

                    Thread.Sleep(TimeSpan.FromMilliseconds(510));
                    var levelTimes = FetchAndPrintLevelTimes();

                    AssertRenderedDuringLastRequest(levelTimes.Level0Duration5);
                    AssertRenderedDuringLastRequest(levelTimes.Level1Duration4);
                    AssertRenderedDuringLastRequest(levelTimes.Level2NoAttribute);
                    AssertRenderedDuringLastRequest(levelTimes.Level3Duration2);
                    AssertRenderedDuringLastRequest(levelTimes.Level4NoAttribute);
                    AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
                });
        }

        private LevelRenderTimes FetchAndPrintLevelTimes()
        {
            var levelTimes = RenderAndFetchLevelTimes();
            PrintDurationsAndCurrentTime(levelTimes);
            return levelTimes;
        }

        private LevelRenderTimes RenderAndFetchLevelTimes()
        {
            return new LevelRenderTimes(ExecuteDefaultAction());
        }

        private void PrintDurationsAndCurrentTime(LevelRenderTimes levelTimes)
        {
            var now = DateTime.Now;

            Console.WriteLine("Time is:            {0}", now.ToString("o"));
            Console.WriteLine();

            Console.WriteLine("Level0 is: {0}", levelTimes.Level0Duration5.ToString("o"));
            Console.WriteLine("Level1 is: {0}", levelTimes.Level1Duration4.ToString("o"));
            Console.WriteLine("Level2 is: {0}", levelTimes.Level2NoAttribute.ToString("o"));
            Console.WriteLine("Level3 is: {0}", levelTimes.Level3Duration2.ToString("o"));
            Console.WriteLine("Level4 is: {0}", levelTimes.Level4NoAttribute.ToString("o"));
            Console.WriteLine("Level5 is: {0}", levelTimes.Level5Duration0.ToString("o"));
            Console.WriteLine();

            Console.WriteLine("Level0Duration5Age is: {0}", (int)(now - levelTimes.Level0Duration5).TotalMilliseconds);
            Console.WriteLine("Level1Duration4Age is: {0}", (int)(now - levelTimes.Level1Duration4).TotalMilliseconds);
            Console.WriteLine("Level2Duration3Age is: {0}", (int)(now - levelTimes.Level2NoAttribute).TotalMilliseconds);
            Console.WriteLine("Level3Duration2Age is: {0}", (int)(now - levelTimes.Level3Duration2).TotalMilliseconds);
            Console.WriteLine("Level4Duration1Age is: {0}", (int)(now - levelTimes.Level4NoAttribute).TotalMilliseconds);
            Console.WriteLine("Level5Duration0Age is: {0}", (int)(now - levelTimes.Level5Duration0).TotalMilliseconds);
            Console.WriteLine();
        }

        private class LevelRenderTimes
        {
            public readonly DateTime Level0Duration5;
            public readonly DateTime Level1Duration4;
            public readonly DateTime Level2NoAttribute;
            public readonly DateTime Level3Duration2;
            public readonly DateTime Level4NoAttribute;
            public readonly DateTime Level5Duration0;

            public LevelRenderTimes(string viewOutPut)
            {
                var levels = viewOutPut.Replace("<br/>", "").Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                Level0Duration5 = DateTime.Parse(levels[0].Split('#')[1]);
                Level1Duration4 = DateTime.Parse(levels[1].Split('#')[1]);
                Level2NoAttribute = DateTime.Parse(levels[2].Split('#')[1]);
                Level3Duration2 = DateTime.Parse(levels[3].Split('#')[1]);
                Level4NoAttribute = DateTime.Parse(levels[4].Split('#')[1]);
                Level5Duration0 = DateTime.Parse(levels[5].Split('#')[1]);
            }
        }
    }
}