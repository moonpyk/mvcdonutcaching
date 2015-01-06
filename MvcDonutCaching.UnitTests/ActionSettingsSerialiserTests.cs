using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Moq;
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

        [Fact]
        public void Serialize_NullActionSettings_ThrowsArgumentNullException()
        {
            // Arrange
            var mockDataContractSerializer = new Mock<IDataContractSerializer>();
            var actionSettingsSerializer = new ActionSettingsSerialiser(mockDataContractSerializer.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => actionSettingsSerializer.Serialise(null));
        }

        [Fact]
        public void Serialize_PopulatedActionSettings_ReturnsSerializedActionSettingAsString()
        {
            //// Arrange
            //var actionSettings = new ActionSettings
            //{
            //    ActionName = "test action name",
            //    ControllerName = "test controller name",
            //    RouteValues = new RouteValueDictionary(new Dictionary<string, object> { { "key", "value" } })
            //};
            //const string expectedResult = "";

            //var mockDataContractSerializer = new Mock<IDataContractSerializer>();
            //mockDataContractSerializer.Setup(d => d.WriteObject(It.IsAny<Stream>(), actionSettings));
            //var actionSettingsSerializer = new ActionSettingsSerialiser(mockDataContractSerializer.Object);

            //// Act
            //string result = actionSettingsSerializer.Serialise(actionSettings);

            //// Assert
            //Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Deserialize_ValidActionSettingsAsString_ReturnsActionSettingsWithCorrectValues()
        {
            // Arrange


            // Act


            // Assert

        }

        [Fact]
        public void Deserialize_InvalidActionSettingsAsString_ReturnsNull()
        {
            // Arrange
            var mockDataContractSerializer = new Mock<IDataContractSerializer>();
            var actionSettingsSerializer = new ActionSettingsSerialiser(mockDataContractSerializer.Object);
            const string invalidActionSettingsAsString = "this text wont deserialize to an ActionSettings";

            // Act
            ActionSettings result = actionSettingsSerializer.Deserialise(invalidActionSettingsAsString);

            // Assert
            Assert.Null(result);
        }
    }
}