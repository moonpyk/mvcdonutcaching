using System;
using System.Threading;
using DevTrends.MvcDonutCaching;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManagedControllerTests : TestsBase
    {
        private const string ControllerUrl = "/FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManaged";

        [Test]
        public void CanExecuteAtAll()
        {
            GetUrlContent(ControllerUrl);
        }

        [Test]
        public void EachLevelReturnsTheSameTimeOnFirstCall()
        {
            var levelTimes = new LevelRenderTimes(GetUrlContent(ControllerUrl));

            AssertRenderedDuringSameRequest(
                levelTimes.Level0Duration5, 
                levelTimes.Level1Duration4, 
                levelTimes.Level2Duration3, 
                levelTimes.Level3Duration2, 
                levelTimes.Level4Duration1, 
                levelTimes.Level5Duration0);
        }


        [Test]
        public void OnlyLevel5HasUpdatedContentAfter100Milliseconds()
        {
            GetUrlContent(ControllerUrl);

            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            var levelTimes = FetchAndPrintLevelTimes();
            AssertRenderedDuringSameRequest(levelTimes.Level0Duration5,levelTimes.Level1Duration4,levelTimes.Level2Duration3,levelTimes.Level3Duration2,levelTimes.Level4Duration1);

            AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
        }

        [Test]
        public void OnlyLevel5And4ShouldHaveCurrentValuesAfter1100Milliseconds()
        {
            GetUrlContent(ControllerUrl);
            
            Thread.Sleep(TimeSpan.FromMilliseconds(1100));
            var levelTimes = FetchAndPrintLevelTimes();
            AssertRenderedDuringSameRequest(levelTimes.Level0Duration5, levelTimes.Level1Duration4, levelTimes.Level2Duration3, levelTimes.Level3Duration2);

            AssertRenderedDuringLastRequest(levelTimes.Level4Duration1);
            AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
        }

        [Test]
        public void Level5_4_and_3ShouldHaveCurrentValuesAfter2100Milliseconds()
        {
            GetUrlContent(ControllerUrl);            

            Thread.Sleep(TimeSpan.FromMilliseconds(2100));
            var levelTimes = FetchAndPrintLevelTimes();

            AssertRenderedDuringSameRequest(levelTimes.Level0Duration5, levelTimes.Level1Duration4, levelTimes.Level2Duration3);

            AssertRenderedDuringLastRequest(levelTimes.Level3Duration2);
            AssertRenderedDuringLastRequest(levelTimes.Level4Duration1);
            AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
        }

        private LevelRenderTimes FetchAndPrintLevelTimes()
        {
            var levelTimes = new LevelRenderTimes(GetUrlContent(ControllerUrl));
            PrintDurationsAndCurrentTime(levelTimes);
            return levelTimes;
        }


        [Test]
        public void Level5_4_3_and2ShouldHaveCurrentValuesAfter3100Milliseconds()
        {
            GetUrlContent(ControllerUrl);            

            Thread.Sleep(TimeSpan.FromMilliseconds(3100));
            var levelTimes = FetchAndPrintLevelTimes();

            AssertRenderedDuringSameRequest(levelTimes.Level0Duration5, levelTimes.Level1Duration4);

            AssertRenderedDuringLastRequest(levelTimes.Level2Duration3);
            AssertRenderedDuringLastRequest(levelTimes.Level3Duration2);
            AssertRenderedDuringLastRequest(levelTimes.Level4Duration1);
            AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
        }


        [Test]
        public void AllButRootShouldHaveCurrentValuesAfter4100Milliseconds()
        {
            GetUrlContent(ControllerUrl);            

            Thread.Sleep(TimeSpan.FromMilliseconds(4100));
            var levelTimes = FetchAndPrintLevelTimes();
            AssertRenderedDuringSameRequest(levelTimes.Level0Duration5);

            AssertRenderedDuringLastRequest(levelTimes.Level1Duration4);
            AssertRenderedDuringLastRequest(levelTimes.Level2Duration3);
            AssertRenderedDuringLastRequest(levelTimes.Level3Duration2);
            AssertRenderedDuringLastRequest(levelTimes.Level4Duration1);
            AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
        }

        [Test]
        public void AllLevelsHaveCurrentValuesAfter5100Milliseconds()
        {
            GetUrlContent(ControllerUrl);

            Thread.Sleep(TimeSpan.FromMilliseconds(5100));
            var levelTimes = FetchAndPrintLevelTimes();

            AssertRenderedDuringLastRequest(levelTimes.Level0Duration5);
            AssertRenderedDuringLastRequest(levelTimes.Level1Duration4);
            AssertRenderedDuringLastRequest(levelTimes.Level2Duration3);
            AssertRenderedDuringLastRequest(levelTimes.Level3Duration2);
            AssertRenderedDuringLastRequest(levelTimes.Level4Duration1);
            AssertRenderedDuringLastRequest(levelTimes.Level5Duration0);
        }

        private void PrintDurationsAndCurrentTime(LevelRenderTimes levelTimes)
        {
            var now = DateTime.Now;

            Console.WriteLine("Time is:            {0}", now.ToString("o"));
            Console.WriteLine();

            Console.WriteLine("Level0Duration5 is: {0}", levelTimes.Level0Duration5.ToString("o"));
            Console.WriteLine("Level1Duration4 is: {0}", levelTimes.Level1Duration4.ToString("o"));
            Console.WriteLine("Level2Duration3 is: {0}", levelTimes.Level2Duration3.ToString("o"));
            Console.WriteLine("Level3Duration2 is: {0}", levelTimes.Level3Duration2.ToString("o"));
            Console.WriteLine("Level4Duration1 is: {0}", levelTimes.Level4Duration1.ToString("o"));
            Console.WriteLine("Level5Duration0 is: {0}", levelTimes.Level5Duration0.ToString("o"));
            Console.WriteLine();

            Console.WriteLine("Level0Duration5Age is: {0}", (int)(now - levelTimes.Level0Duration5).TotalMilliseconds);
            Console.WriteLine("Level1Duration4Age is: {0}", (int)(now - levelTimes.Level1Duration4).TotalMilliseconds);
            Console.WriteLine("Level2Duration3Age is: {0}", (int)(now - levelTimes.Level2Duration3).TotalMilliseconds);
            Console.WriteLine("Level3Duration2Age is: {0}", (int)(now - levelTimes.Level3Duration2).TotalMilliseconds);
            Console.WriteLine("Level4Duration1Age is: {0}", (int)(now - levelTimes.Level4Duration1).TotalMilliseconds);
            Console.WriteLine("Level5Duration0Age is: {0}", (int)(now - levelTimes.Level5Duration0).TotalMilliseconds);
            Console.WriteLine();
        }

        private class LevelRenderTimes
        {
            public readonly DateTime Level0Duration5;
            public readonly DateTime Level1Duration4;
            public readonly DateTime Level2Duration3;
            public readonly DateTime Level3Duration2;
            public readonly DateTime Level4Duration1;
            public readonly DateTime Level5Duration0;
            public LevelRenderTimes(string viewOutPut)
            {
                for (int i = 0; i < 7; i++)
                {
                    viewOutPut = DonutHoleFiller.RemoveDonutHoleWrappers(viewOutPut);
                }

                var levels = viewOutPut.Replace("<br/>", "").Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                Level0Duration5 = DateTime.Parse(levels[0].Split('#')[1]);
                Level1Duration4 = DateTime.Parse(levels[1].Split('#')[1]);
                Level2Duration3 = DateTime.Parse(levels[2].Split('#')[1]);
                Level3Duration2 = DateTime.Parse(levels[3].Split('#')[1]);
                Level4Duration1 = DateTime.Parse(levels[4].Split('#')[1]);
                Level5Duration0 = DateTime.Parse(levels[5].Split('#')[1]);
            }
        }
    }
}