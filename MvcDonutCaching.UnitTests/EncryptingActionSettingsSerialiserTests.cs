using System;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Moq;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class EncryptingActionSettingsSerialiserTests
    {
        [Fact]
        public void Constructor_NullValues_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EncryptingActionSettingsSerialiser(null, null));
            Assert.Throws<ArgumentNullException>(() => new EncryptingActionSettingsSerialiser(new ActionSettingsSerialiser(), null));
            Assert.Throws<ArgumentNullException>(() => new EncryptingActionSettingsSerialiser(null, new Encryptor()));
        }

        [Fact]
        public void Serialize_NullActionSettings_ThrowsArgumentNullException()
        {
            // Arrange
            var mockActionSettingsSerializer = new Mock<IActionSettingsSerialiser>();
            var mockEncryptor = new Mock<IEncryptor>();

            var encryptingActionSettingsSerializer =
                new EncryptingActionSettingsSerialiser(mockActionSettingsSerializer.Object, mockEncryptor.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => encryptingActionSettingsSerializer.Serialise(null));
        }

        [Fact]
        public void Serialize_ValidActionSettings_ActionSettingSerializedAndEncrypted()
        {
            // Arrange
            var routeValueDictionary = new RouteValueDictionary { { "locale", "en-US" } };
            var actionSettings = new ActionSettings
            {
                ActionName = "MyAction",
                ControllerName = "MyController",
                RouteValues = routeValueDictionary
            };

            const string expectedActionSettings = "this is a test serialzed action setting";
            var mockActionSettingsSerializer = new Mock<IActionSettingsSerialiser>();
            mockActionSettingsSerializer.Setup(s => s.Serialise(actionSettings)).Returns(expectedActionSettings);

            const string encryptedActionSetting = "encrypted result";
            var mockEncryptor = new Mock<IEncryptor>();
            mockEncryptor.Setup(e => e.Encrypt(expectedActionSettings)).Returns(encryptedActionSetting);

            var encryptingActionSettingsSerializer =
                new EncryptingActionSettingsSerialiser(mockActionSettingsSerializer.Object, mockEncryptor.Object);

            // Act
            string serializedActionSettings = encryptingActionSettingsSerializer.Serialise(actionSettings);

            // Assert
            Assert.Equal(encryptedActionSetting, serializedActionSettings);
            mockActionSettingsSerializer.Verify(s => s.Serialise(actionSettings), Times.Once);
            mockEncryptor.Verify(e => e.Encrypt(expectedActionSettings), Times.Once);
        }

        [Fact]
        public void Deserialize_ValidString_StringDecryptedAndDeserialized()
        {
            // Arrange
            var routeValueDictionary = new RouteValueDictionary { { "locale", "en-US" } };
            var testActionSettings = new ActionSettings
            {
                ActionName = "MyAction",
                ControllerName = "MyController",
                RouteValues = routeValueDictionary
            };

            const string expectedActionSettings = "this is a test serialzed action setting";
            const string decryptedActionSetting = "decrypted result";

            var mockActionSettingsSerializer = new Mock<IActionSettingsSerialiser>();
            mockActionSettingsSerializer.Setup(s => s.Deserialise(decryptedActionSetting)).Returns(testActionSettings);
            
            var mockEncryptor = new Mock<IEncryptor>();
            mockEncryptor.Setup(e => e.Decrypt(expectedActionSettings)).Returns(decryptedActionSetting);

            var encryptingActionSettingsSerializer =
                new EncryptingActionSettingsSerialiser(mockActionSettingsSerializer.Object, mockEncryptor.Object);

            // Act
            ActionSettings actionSettings = encryptingActionSettingsSerializer.Deserialise(expectedActionSettings);

            // Assert
            Assert.Equal(actionSettings, testActionSettings);
            mockActionSettingsSerializer.Verify(s => s.Deserialise(decryptedActionSetting), Times.Once);
            mockEncryptor.Verify(e => e.Decrypt(expectedActionSettings), Times.Once);
        }
    }
}