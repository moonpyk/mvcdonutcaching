using System;
using System.Net;
using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture, ExclusivelyUses(Controller), Isolated]//Applies so much load that it will screw up the timing for other tests.
    public class ProvokeRaceCondition1Tests : ControllerTestBase
    {
        private const string Controller = "ProvokeRaceCondition1";
        override protected string ControllerName { get { return Controller; } }

        [Test]
        public void KeepsRenderingIdenticalContentWhenMultipleThreadsHammerRequests()
        {
            Console.WriteLine("Starting");

            long requestsMade = 0;
            try
            {
                ThreadingHelper.HammerMultiThreadedFor(
                    TimeSpan.FromMilliseconds(1000),
                    out requestsMade, () => ExecuteDefaultAction());
            }
            catch(AggregateException exceptions)
            {
                exceptions.Handle(e =>
                                  {
                                      if(e.InnerException is WebException)
                                      {
                                          Console.WriteLine("Server side error message:");
                                          Console.WriteLine(e.Message);
                                      }
                                      return false;
                                  });
            }

            Console.WriteLine("Made {0} requests without errors :)", requestsMade);
        }
    }
}
