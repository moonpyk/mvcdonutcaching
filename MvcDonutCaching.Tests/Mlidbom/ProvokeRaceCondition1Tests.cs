using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture, ExclusivelyUses(Controller)]
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
