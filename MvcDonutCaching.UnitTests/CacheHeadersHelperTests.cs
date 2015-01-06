using System;
using System.Web;
using DevTrends.MvcDonutCaching;
using Moq;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class CacheHeadersHelperTests
    {
        [Fact]
        public void SetCacheHeaders_NullParameters_ThrowsArgumentNullException()
        {
            // Arrange
            var cacheHeadersHelper = new CacheHeadersHelper();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => cacheHeadersHelper.SetCacheHeaders(null, null));
            Assert.Throws<ArgumentNullException>(() => cacheHeadersHelper.SetCacheHeaders(null, new CacheSettings()));
            Assert.Throws<ArgumentNullException>(() => cacheHeadersHelper.SetCacheHeaders(new HttpResponseWrapper(new HttpResponse(null)), null));
        }

        [Fact]
        public void SetCacheHeaders_SettingsLocationAny_HttpCacheabilityPublic()
        {
            // Arrange
            var cacheHeadersHelper = new CacheHeadersHelper();
            var httpResponseBase = new Mock<HttpResponseBase>();
            httpResponseBase.Setup(x => x.Cache.SetCacheability(It.IsAny<HttpCacheability>()));
            var cacheSettings = new CacheSettings();

            // Act
            cacheHeadersHelper.SetCacheHeaders(httpResponseBase.Object, cacheSettings);

            // Assert
            //Assert.Equal(HttpCacheability.Public, httpResponseBase.Headers);
        }

        // OutputCacheLocation.Any OR .Downstream = Public
        // .Client or .ServerAndClient = Private
        // .Server or .None = NoCache
        // if it isnt NoCache (use .Any), then SetExpires should be X and SetMaxAge should be Y
        // if settings.NoStore then whatever the result of response.Cache.SetNoStore()
    }
}