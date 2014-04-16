using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvcDonutCaching.Tests.Mlidbom
{
    public static class ThreadingHelper
    {
        public static void HammerMultiThreadedFor(TimeSpan duration, out long requestsMade, Action hammer)
        {
            long executions = 0;

            var cancellationTokenSource = new CancellationTokenSource();
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = cancellationTokenSource.Token,
                MaxDegreeOfParallelism = 20
            };

            var runUntil = DateTime.Now + duration;
            try
            {
                Parallel.For(1,
                    int.MaxValue,
                    parallelOptions,
                    _ =>
                    {
                        Interlocked.Increment(ref executions);
                        if(runUntil < DateTime.Now)
                        {
                            cancellationTokenSource.Cancel();
                        }
                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();

                        hammer();
                    });
            }
            catch(OperationCanceledException) {}
            finally
            {
                requestsMade = executions;
            }
        }
    }
}