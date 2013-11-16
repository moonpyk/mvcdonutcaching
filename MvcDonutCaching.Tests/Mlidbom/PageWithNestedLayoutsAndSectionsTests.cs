using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    //Render order is all sorts of funny in this scenario :)
    [TestFixture, ExclusivelyUses(Controller)]
    public class PageWithNestedLayoutsAndSectionsTests : ControllerTestBase
    {        
        private const string Controller = "PageWithNestedLayoutsAndSections";
        override protected string ControllerName { get { return Controller; } }

        [Test]
        public void RendersIdenticalContentFromCacheAsOnInitialRender()
        {
            var initialContent = ExecuteDefaultAction();
            var cachedContent = ExecuteDefaultAction();

            Assert.That(initialContent, Is.EqualTo(cachedContent));
        }
    }
}