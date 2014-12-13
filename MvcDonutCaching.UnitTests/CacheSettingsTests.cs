using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    /// <summary>
    /// Tests for the CacheSettings class.
    /// </summary>
    public class CacheSettingsTests
    {
        [Fact]
        public void CorrectFieldsExist()
        {
            // Arrange
            const bool isCachingEnabled = true;
            const int duration = 1;
            const string varyByParam = "none";
            const string varyByCustom = "all";
            const OutputCacheLocation location = OutputCacheLocation.None;
            const bool noStore = true;
            const OutputCacheOptions options = OutputCacheOptions.None;
            const bool isServerCachingEnabled = false;

            // Act
            var cacheSettings = new CacheSettings()
            {
                IsCachingEnabled = isCachingEnabled,
                Duration = duration,
                VaryByParam = varyByParam,
                VaryByCustom = varyByCustom,
                Location = location,
                NoStore = noStore,
                Options = options
            };

            // Assert
            Assert.Equal(isCachingEnabled, cacheSettings.IsCachingEnabled);
            Assert.Equal(duration, cacheSettings.Duration);
            Assert.Equal(varyByParam, cacheSettings.VaryByParam);
            Assert.Equal(varyByCustom, cacheSettings.VaryByCustom);
            Assert.Equal(location, cacheSettings.Location);
            Assert.Equal(noStore, cacheSettings.NoStore);
            Assert.Equal(options, cacheSettings.Options);
            Assert.Equal(isServerCachingEnabled, cacheSettings.IsServerCachingEnabled);
        }

        [Fact]
        public void IsServerCachingEnabled_CachingNotEnabled_ReturnsFalse()
        {
            // Arrange
            const bool isCachingEnabled = false;
            const int duration = 1;
            const string varyByParam = "none";
            const string varyByCustom = "all";
            const OutputCacheLocation location = OutputCacheLocation.None;
            const bool noStore = true;
            const OutputCacheOptions options = OutputCacheOptions.None;
            const bool isServerCachingEnabled = false;

            // Act
            var cacheSettings = new CacheSettings()
            {
                IsCachingEnabled = isCachingEnabled,
                Duration = duration,
                VaryByParam = varyByParam,
                VaryByCustom = varyByCustom,
                Location = location,
                NoStore = noStore,
                Options = options
            };

            // Assert
            Assert.Equal(isCachingEnabled, cacheSettings.IsCachingEnabled);
            Assert.Equal(duration, cacheSettings.Duration);
            Assert.Equal(varyByParam, cacheSettings.VaryByParam);
            Assert.Equal(varyByCustom, cacheSettings.VaryByCustom);
            Assert.Equal(location, cacheSettings.Location);
            Assert.Equal(noStore, cacheSettings.NoStore);
            Assert.Equal(options, cacheSettings.Options);
            Assert.Equal(isServerCachingEnabled, cacheSettings.IsServerCachingEnabled);
        }

        [Fact]
        public void IsServerCachingEnabled_DurationZero_ReturnsFalse()
        {
            // Arrange
            const bool isCachingEnabled = true;
            const int duration = 0;
            const string varyByParam = "none";
            const string varyByCustom = "all";
            const OutputCacheLocation location = OutputCacheLocation.Any;
            const bool noStore = true;
            const OutputCacheOptions options = OutputCacheOptions.None;
            const bool isServerCachingEnabled = false;

            // Act
            var cacheSettings = new CacheSettings()
            {
                IsCachingEnabled = isCachingEnabled,
                Duration = duration,
                VaryByParam = varyByParam,
                VaryByCustom = varyByCustom,
                Location = location,
                NoStore = noStore,
                Options = options
            };

            // Assert
            Assert.Equal(isCachingEnabled, cacheSettings.IsCachingEnabled);
            Assert.Equal(duration, cacheSettings.Duration);
            Assert.Equal(varyByParam, cacheSettings.VaryByParam);
            Assert.Equal(varyByCustom, cacheSettings.VaryByCustom);
            Assert.Equal(location, cacheSettings.Location);
            Assert.Equal(noStore, cacheSettings.NoStore);
            Assert.Equal(options, cacheSettings.Options);
            Assert.Equal(isServerCachingEnabled, cacheSettings.IsServerCachingEnabled);
        }

        [Fact]
        public void IsServerCachingEnabled_OutputCacheLocationNone_ReturnsFalse()
        {
            // Arrange
            const bool isCachingEnabled = true;
            const int duration = 1;
            const string varyByParam = "none";
            const string varyByCustom = "all";
            const OutputCacheLocation location = OutputCacheLocation.None;
            const bool noStore = true;
            const OutputCacheOptions options = OutputCacheOptions.None;
            const bool isServerCachingEnabled = false;

            // Act
            var cacheSettings = new CacheSettings()
            {
                IsCachingEnabled = isCachingEnabled,
                Duration = duration,
                VaryByParam = varyByParam,
                VaryByCustom = varyByCustom,
                Location = location,
                NoStore = noStore,
                Options = options
            };

            // Assert
            Assert.Equal(isCachingEnabled, cacheSettings.IsCachingEnabled);
            Assert.Equal(duration, cacheSettings.Duration);
            Assert.Equal(varyByParam, cacheSettings.VaryByParam);
            Assert.Equal(varyByCustom, cacheSettings.VaryByCustom);
            Assert.Equal(location, cacheSettings.Location);
            Assert.Equal(noStore, cacheSettings.NoStore);
            Assert.Equal(options, cacheSettings.Options);
            Assert.Equal(isServerCachingEnabled, cacheSettings.IsServerCachingEnabled);
        }

        [Fact]
        public void IsServerCachingEnabled_CachingEnabledDurationOneOutputCacheLocationAny_ReturnsTrue()
        {
            // Arrange
            const bool isCachingEnabled = true;
            const int duration = 1;
            const string varyByParam = "none";
            const string varyByCustom = "all";
            const OutputCacheLocation location = OutputCacheLocation.Any;
            const bool noStore = true;
            const OutputCacheOptions options = OutputCacheOptions.None;
            const bool isServerCachingEnabled = true;

            // Act
            var cacheSettings = new CacheSettings()
            {
                IsCachingEnabled = isCachingEnabled,
                Duration = duration,
                VaryByParam = varyByParam,
                VaryByCustom = varyByCustom,
                Location = location,
                NoStore = noStore,
                Options = options
            };

            // Assert
            Assert.Equal(isCachingEnabled, cacheSettings.IsCachingEnabled);
            Assert.Equal(duration, cacheSettings.Duration);
            Assert.Equal(varyByParam, cacheSettings.VaryByParam);
            Assert.Equal(varyByCustom, cacheSettings.VaryByCustom);
            Assert.Equal(location, cacheSettings.Location);
            Assert.Equal(noStore, cacheSettings.NoStore);
            Assert.Equal(options, cacheSettings.Options);
            Assert.Equal(isServerCachingEnabled, cacheSettings.IsServerCachingEnabled);
        }
    }
}