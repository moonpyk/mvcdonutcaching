using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Web.Mvc;
using MvcDonutCaching.Tests.Web;
using MvcDonutCaching.Tests.Web.Controllers;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class CachingControllerWithNoChildActionsTests
    {

        public static string GetUrlContent(string relativeUrl)
        {
            var uri = string.Format("http://localhost:{0}{1}",49421, relativeUrl);
            var webRequest = WebRequest.Create(uri);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

            using (var response = webRequest.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [Test]
        public void CanExecuteAtAll()
        {
            var result = GetUrlContent("/CachingControllerWithNoChildActions");
            Console.WriteLine(result);
        }
    }
}