﻿using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;

namespace DevTrends.MvcDonutCaching.Demo.Controllers
{
    public class LoadTestController : Controller
    {
        public ActionResult ApplyLoad()
        {
            Console.WriteLine("Starting");

            long requestsMade = 0;

            var relativeUrl = Url.Action("LargeOutPutRootAction");
            var uri = string.Format("{0}://{1}:{2}{3}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, relativeUrl);

            var cancellationTokenSource = new CancellationTokenSource();
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = cancellationTokenSource.Token,
                MaxDegreeOfParallelism = 20
            };

            var runUntil = DateTime.Now.AddSeconds(10);

            try
            {
                Parallel.For(1,
                    int.MaxValue,
                    parallelOptions,
                    _ =>
                    {
                        if (runUntil < DateTime.Now)
                        {
                            cancellationTokenSource.Cancel();
                        }
                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();

                        var webRequest = WebRequest.Create(uri);
                        webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                        using (var response = webRequest.GetResponse())
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            reader.ReadToEnd();
                            Interlocked.Increment(ref requestsMade);
                        }
                    });
            }
            catch (OperationCanceledException)
            {
            }

            return View(requestsMade);
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult LargeOutPutRootAction()
        {
            return View(DateTime.Now);
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult MediumOutPutChildAction()
        {
            return PartialView(DateTime.Now);
        }

        [AutoOutputCache(Duration = 3600)]
        public ActionResult SmallOutPutGrandChildAction()
        {
            return PartialView(DateTime.Now);
        }
    }
}
