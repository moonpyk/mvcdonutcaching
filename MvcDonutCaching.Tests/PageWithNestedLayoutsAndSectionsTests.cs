using System.Web.Caching;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    //Render order is all sorts of funny in this scenario :)
    [TestFixture]
    public class PageWithNestedLayoutsAndSectionsTests : TestsBase
    {
        private const string ActionPath = "/PageWithNestedLayoutsAndSections";

        [Test]
        public void RendersIdenticalContentFromCacheAsOnInitialRender()
        {
            var initialContent = GetUrlContent(ActionPath);
            var cachedContent = GetUrlContent(ActionPath);

            Assert.That(initialContent, Is.EqualTo(cachedContent));
        }
    }
}