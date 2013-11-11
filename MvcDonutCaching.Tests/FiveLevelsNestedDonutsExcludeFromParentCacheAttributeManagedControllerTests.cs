using System;
using System.Threading;
using DevTrends.MvcDonutCaching;
using NUnit.Framework;

namespace MvcDonutCaching.Tests
{
    [TestFixture]
    public class FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManagedControllerTests : FiveLevelsNestedDonutsControllerTests
    {
        protected override string ControllerUrl
        {
            get { return "/FiveLevelsNestedDonutsExcludeFromParentCacheAttributeManaged"; }
        }       
    }
}