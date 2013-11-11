using System;

namespace MvcDonutCaching.Tests.Web
{
    public class TestTimeSource
    {
        public static TestTimeSource Instance = new TestTimeSource();
        public DateTime Now = DateTime.Now;
    }
}