using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class Caching1SecondControllerWithNoChildActionsTests
    {
        [SetUp]
        public void SetupTask()
        {
            ClearCache();
        }

        private static void ClearCache()
        {
            var result = GetUrlContent("/TestControl/ClearCache");
            Assert.That(result, Is.EqualTo("Done"), "Failed to clear cache");
        }

        public static string GetUrlContent(string relativeUrl)
        {
            var uri = string.Format("http://localhost:{0}{1}",49421, relativeUrl);
            var webRequest = WebRequest.Create(uri);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

            try
            {
                using(var response = webRequest.GetResponse())
                return ReadResponse(response);
            }
            catch(WebException e)
            {
                string responseText = null;
                try
                {
                    responseText = ReadResponse(e.Response);
                }
                catch(Exception inner)
                {
                    
                }
                if(responseText != null)
                {
                    throw new Exception(responseText, e);
                }
                throw;
            }  
        }

        private static string ReadResponse(WebResponse response)
        {
            using(var stream = response.GetResponseStream())
            {
                using(var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        [Test]
        public void CanExecuteAtAll()
        {
            var result = GetUrlContent("/Caching1SecondControllerWithNoChildActions");
            Console.WriteLine(result);
        }

        [Test]
        public void CallingTwiceWith100MillisecondsApartReturnsIdenticalResults()
        {
            var result1 = GetUrlContent("/Caching1SecondControllerWithNoChildActions");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            var result100MillisecondsLater = GetUrlContent("/Caching1SecondControllerWithNoChildActions");
            Assert.That(result1, Is.EqualTo(result100MillisecondsLater));
        }

        [Test]
        public void CallingTwiceWith2SecondsApartReturnsDifferentResults()
        {
            var result1 = GetUrlContent("/Caching1SecondControllerWithNoChildActions");
            Thread.Sleep(TimeSpan.FromMilliseconds(2000));
            var result2SecondsLater = GetUrlContent("/Caching1SecondControllerWithNoChildActions");
            Assert.That(result1, Is.Not.EqualTo(result2SecondsLater));
        }
    }
}