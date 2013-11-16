using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    public class MultipleIdenticalChildActionCalls : TestsBase
    {
        private const string ControllerActionPath = "/MultipleIdenticalChildActionCalls";

        private const string CorrectOutput = 
@"Root
<div>ChildAction</div>
<div>ChildAction</div>
<div>ChildAction</div>
<div>ChildAction</div>";

        [Test]
        public void CanRenderAtAll()
        {
            var result = GetUrlContent(ControllerActionPath);
            Assert.That(result, Is.EqualTo(CorrectOutput));
        }

        [Test]
        public void CachedResultIsIdenticalToOriginalResult()
        {
            var originalResult = GetUrlContent(ControllerActionPath);
            var cachedResult = GetUrlContent(ControllerActionPath);
            Assert.That(cachedResult, Is.EqualTo(originalResult));
        }
    }
}