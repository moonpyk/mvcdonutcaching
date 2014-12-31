using System;
using DevTrends.MvcDonutCaching;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class ActionSettingsSerialiserTests
    {
        [Fact]
        public void Constructor_NullDataContractSerializer_ThrowsArgumentNullException()
        {
            // Arrange and Act and Assert
            Assert.Throws<ArgumentNullException>(() => new ActionSettingsSerialiser(null));
        }
    }
}