using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Moq;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class KeyGeneratorTests
    {
        [Fact]
        public void Constructor_NullKeyBuilder_ThrowsArgumentNullException()
        {
            // Arrange and Act and Assert
            Assert.Throws<ArgumentNullException>(() => new KeyGenerator(null));
        }

        [Fact]
        public void GenerateKey_NullParameters_ThrowsArgumentNullException()
        {
            // Arrange
            var mockKeyBuilder = new Mock<IKeyBuilder>();
            var keyGenerator = new KeyGenerator(mockKeyBuilder.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => keyGenerator.GenerateKey(null, null));
            Assert.Throws<ArgumentNullException>(() => keyGenerator.GenerateKey(null, new CacheSettings()));
            Assert.Throws<ArgumentNullException>(() => keyGenerator.GenerateKey(new ControllerContext(), null));
        }
    }
}
