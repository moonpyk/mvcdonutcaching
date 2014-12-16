using System;
using System.Web;
using DevTrends.MvcDonutCaching;
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
    }
}