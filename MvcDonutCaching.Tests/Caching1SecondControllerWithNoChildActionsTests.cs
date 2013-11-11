using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class Caching100MillisecondsControllerWithNoChildActionsTests : TestsBase
    {
        private const string ControllerUrl = "/Caching1SecondControllerWithNoChildActions";

        [Test]
        public void CanExecuteAtAll()
        {
            var result = GetUrlContent(ControllerUrl);
            Console.WriteLine(result);
        }

        [Test]
        public void CallingTwiceWith50MillisecondsApartReturnsIdenticalResults()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var result1 = GetUrlContent(ControllerUrl);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    var result100MillisecondsLater = GetUrlContent(ControllerUrl);
                    Assert.That(result1, Is.EqualTo(result100MillisecondsLater));
                });
        }

        [Test]
        public void CallingTwiceWith200MillisecondsApartReturnsDifferentResults()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var result1 = GetUrlContent(ControllerUrl);
                    Thread.Sleep(TimeSpan.FromMilliseconds(200));
                    var result2SecondsLater = GetUrlContent(ControllerUrl);
                    Assert.That(result1, Is.Not.EqualTo(result2SecondsLater));
                });
        }
    }
}