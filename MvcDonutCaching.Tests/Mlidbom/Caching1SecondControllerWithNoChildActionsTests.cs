using System;
using System.Threading;
using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture, ExclusivelyUses(Controller)]
    public class Caching100MillisecondsControllerWithNoChildActionsTests : ControllerTestBase
    {
        private const string Controller = "Caching1SecondWithNoChildActions";
        override protected string ControllerName { get { return Controller; } }

        [Test]
        public void CanExecuteAtAll()
        {
            var result = ExecuteDefaultAction();
            Console.WriteLine(result);
        }

        [Test]
        public void CallingTwiceWith50MillisecondsApartReturnsIdenticalResults()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var result1 = ExecuteDefaultAction();
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    var result100MillisecondsLater = ExecuteDefaultAction();
                    Assert.That(result1, Is.EqualTo(result100MillisecondsLater));
                });
        }

        [Test]
        public void CallingTwiceWith200MillisecondsApartReturnsDifferentResults()
        {
            RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(
                () =>
                {
                    var result1 = ExecuteDefaultAction();
                    Thread.Sleep(TimeSpan.FromMilliseconds(200));
                    var result2SecondsLater = ExecuteDefaultAction();
                    Assert.That(result1, Is.Not.EqualTo(result2SecondsLater));
                });
        }        
    }
}