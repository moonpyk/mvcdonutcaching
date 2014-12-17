using DevTrends.MvcDonutCaching;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class CacheItemTests
    {
        [Fact]
        public void CorrectFieldsExist()
        {
            // Arrange
            const string contentType = "content_type";
            const string content = "content";
            var cacheItem = new CacheItem();

            // Act
            cacheItem.ContentType = contentType;
            cacheItem.Content = content;

            // Assert
            Assert.Equal(contentType, cacheItem.ContentType);
            Assert.Equal(content, cacheItem.Content);
        }
    }
}