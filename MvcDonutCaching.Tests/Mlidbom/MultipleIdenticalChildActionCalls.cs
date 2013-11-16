using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture, ExclusivelyUses(Controller)]
    public class MultipleIdenticalChildActionCalls : ControllerTestBase
    {        
        private const string Controller = "MultipleIdenticalChildActionCalls";
        override protected string ControllerName { get { return Controller; } }

        private const string CorrectOutput = 
@"Root
<div>ChildAction</div>
<div>ChildAction</div>
<div>ChildAction</div>
<div>ChildAction</div>";

        [Test]
        public void CanRenderAtAll()
        {
            var result = ExecuteDefaultAction();
            Assert.That(result, Is.EqualTo(CorrectOutput));
        }

        [Test]
        public void CachedResultIsIdenticalToOriginalResult()
        {
            var originalResult = ExecuteDefaultAction();
            var cachedResult = ExecuteDefaultAction();
            Assert.That(cachedResult, Is.EqualTo(originalResult));
        }
    }
}