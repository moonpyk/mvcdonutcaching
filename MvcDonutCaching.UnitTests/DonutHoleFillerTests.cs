using System;
using DevTrends.MvcDonutCaching;
using Moq;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class DonutHoleFillerTests
    {
        [Fact]
        public void Constructor_NullActionSettingsSerializer_ThrowsArgumentNullException()
        {
            // Arrange and Act and Assert
            Assert.Throws<ArgumentNullException>(() => new DonutHoleFiller(null));
        }

        [Fact]
        public void RemoveDonutHoleWrappers_NullControllerContext_ThrowsArgumentNullException()
        {
            // Arrange
            var mockActionSettingsSerializer = new Mock<IActionSettingsSerialiser>();
            var donutHoleFiller = new DonutHoleFiller(mockActionSettingsSerializer.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => donutHoleFiller.RemoveDonutHoleWrappers("content", null, OutputCacheOptions.None));
        }
    }
}
