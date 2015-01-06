using System.Collections.Generic;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class KeyBuilderTests
    {
        [Fact]
        public void GetCacheKeyPrefix_ReturnsCorrectCacheKeyPrefix()
        {
            // Arrange
            const string expectedCacheKey = "_d0nutc@che.";
            var keyBuilder = new KeyBuilder();

            // Act
            string cacheKeyPrefix = keyBuilder.CacheKeyPrefix;

            // Assert
            Assert.Equal(expectedCacheKey, cacheKeyPrefix);
        }

        [Fact]
        public void BuildKey_ControllerNameActionNameRouteValues_CorrectlyFormattedKey()
        {
            // Arrange
            const string controllerName = "myController";
            const string actionName = "myAction";
            var routeValueDictionary = new RouteValueDictionary(new Dictionary<string, object> {{"x", "y"}, {"i", "j"}});
            const string expectedKey = "_d0nutc@che.mycontroller.myaction#x=y#i=j#";
            var keyBuilder = new KeyBuilder();

            // Act
            string key = keyBuilder.BuildKey(controllerName, actionName, routeValueDictionary);

            // Assert
            Assert.Equal(expectedKey, key);
        }

        [Fact]
        public void BuildKey_OnlyControllerName_CorrectlyFormattedKey()
        {
            // Arrange
            const string controllerName = "myController";
            const string expectedKey = "_d0nutc@che.mycontroller.";
            var keyBuilder = new KeyBuilder();

            // Act
            string key = keyBuilder.BuildKey(controllerName);

            // Assert
            Assert.Equal(expectedKey, key);
        }

        [Fact]
        public void BuildKey_ControllerNameActionName_CorrectlyFormattedKey()
        {
            // Arrange
            const string controllerName = "myController";
            const string actionName = "myAction";
            const string expectedKey = "_d0nutc@che.mycontroller.myaction#";
            var keyBuilder = new KeyBuilder();

            // Act
            string key = keyBuilder.BuildKey(controllerName, actionName);

            // Assert
            Assert.Equal(expectedKey, key);
        }

        [Fact]
        public void BuildKeyFragment_RouteValues_CorrectlyFormattedKey()
        {
            // Arrange
            var routeValues = new KeyValuePair<string, object>("key", "value");
            const string expectedKey = "key=value#";
            var keyBuilder = new KeyBuilder();

            // Act
            string key = keyBuilder.BuildKeyFragment(routeValues);

            // Assert
            Assert.Equal(expectedKey, key);
        }

        [Fact]
        public void BuildKeyFragment_NullRouteValue_CorrectlyFormattedNullKey()
        {
            // Arrange
            var routeValues = new KeyValuePair<string, object>("key", null);
            const string expectedKey = "key=<null>#";
            var keyBuilder = new KeyBuilder();

            // Act
            string key = keyBuilder.BuildKeyFragment(routeValues);

            // Assert
            Assert.Equal(expectedKey, key);
        }
    }
}