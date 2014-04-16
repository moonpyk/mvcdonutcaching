using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{

    public abstract class ControllerTestBase 
    {
        private static readonly string TestSiteBaseUrl = ConfigurationManager.AppSettings["TestSiteBaseUrl"];

        protected abstract string ControllerName { get; }
        [SetUp]
        public virtual void RunBeforeEachTest()
        {
            ClearCache();
        }

        public virtual void RunAfterEachTest()
        {
            ClearCache();
        }

        [TearDown]
        public void TearDownTask()
        {
            UseDefaultSettingsGlobally();
        }

        protected void RetryThreeTimesOnFailureSinceTimingIssuesWithTheWebServerAndStartUpMayCauseIntermittentFailures(Action test)
        {
            for(int tries = 0; tries <= 3; tries++)
            {
                try
                {
                    RunBeforeEachTest();
                    test();
                    RunAfterEachTest();
                    return;
                }
                catch(Exception)
                {
                    if(tries == 3)
                    {
                        throw;
                    }
                }
            }
        }

        protected string ExecuteDefaultAction()
        {
            return GetUrlContent("/{0}", ControllerName);
        }

        protected string ExecuteAction(string action)
        {
            return GetUrlContent("/{0}/{1}", ControllerName, action);
        }

        protected void AssertRenderedDuringSameRequest(
            params DateTime[] times)
        {
            var baseTime = times.First();
            foreach(var dateTime in times)
            {
                Assert.That(dateTime, Is.EqualTo(baseTime).Within(TimeSpan.FromMilliseconds(100)));
            }
        }


        protected void AssertRenderedDuringLastRequest(
            params DateTime[] times)
        {
            var baseTime = DateTime.Now;
            foreach (var dateTime in times)
            {
                Assert.That(dateTime, Is.EqualTo(baseTime).Within(TimeSpan.FromMilliseconds(100)));
            }
        }

        protected void CleanAllCache()
        {
            var result = GetUrlContent("/TestControl/ClearCache");
            Assert.That(result, Is.EqualTo("Done"), "Failed to clear cache");
        }

        protected void ClearControllerCache(string controllerName)
        {
            var result = GetUrlContent("/TestControl/ClearCache?controllerName={0}", controllerName);
            Assert.That(result, Is.EqualTo("Done"), "Failed to clear cache");
        }

        protected void ClearActionCache(string controllerName, string actionName)
        {
            var result = GetUrlContent("/TestControl/ClearCache?controllerName={0}&actionName={1}", controllerName, actionName);
            Assert.That(result, Is.EqualTo("Done"), "Failed to clear cache");
        }

        protected virtual void ClearCache()
        {
            ClearControllerCache(ControllerName);
        }

        protected static void EnableReplaceDonutsInChildActionsGlobally()
        {
            var result = GetUrlContent("/TestControl/EnableReplaceDonutsInChildActionsGlobally");
            Assert.That(result, Is.EqualTo("Done"), "Failed:EnableReplaceDonutsInChildActionsGlobally");
        }

        private static void UseDefaultSettingsGlobally()
        {
            var result = GetUrlContent("/TestControl/UseDefaultSettingsGlobally");
            Assert.That(result, Is.EqualTo("Done"), "Failed:UseDefaultSettingsGlobally");
        }

        public static string GetUrlContent(string relativeUrl, params object[] formatValues)
        {
            relativeUrl = string.Format(relativeUrl, formatValues);
            var uri = string.Format("{0}{1}", TestSiteBaseUrl, relativeUrl);
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
                catch(Exception)
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
    }
}