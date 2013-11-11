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
        [SetUp]
        public void SetupTask()
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
            var result = GetUrlContent("/CachingControllerWithNoChildActions");
            Console.WriteLine(result);
        }
    }
}