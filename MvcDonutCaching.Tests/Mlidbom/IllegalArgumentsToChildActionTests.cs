using System;
using System.Net;
using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture, ExclusivelyUses(Controller)]
    public abstract class IllegalArgumentsToChildActionTests : ControllerTestBase
    {
        private const string ThrownExceptionText = "System.ArgumentException: The parameters dictionary contains a null entry for parameter 'theParameter' of non-nullable type 'System.Int32'";
        public const string Controller = "IllegalArgumentsToChildAction";
        protected abstract string ThrowingAction { get; }

        override protected string ControllerName { get { return Controller; } }

        [Test]
        public void ControllerReturns500ErrorOnEveryCall()
        {
            //UnCached
            var thrown = (WebException)Assert.Throws<Exception>(() => ExecuteAction()).InnerException;
            Console.WriteLine(thrown);
            Console.WriteLine(thrown.Status);
            //Cached
            thrown = (WebException)Assert.Throws<Exception>(() => ExecuteAction()).InnerException;
        }

        [Test]
        public void OutputAlwaysContainTheExceptionMessage()
        {
            //uncached
            var exception = Assert.Throws<Exception>(() => ExecuteAction());
            var output = exception.Message;
            Assert.That(output, Contains.Substring(ThrownExceptionText));

            //cached
            exception = Assert.Throws<Exception>(() => ExecuteAction());
            output = exception.Message;
            Assert.That(output, Contains.Substring(ThrownExceptionText));
        }

        private string ExecuteAction()
        {
            return ExecuteAction(ThrowingAction);
        }
    }

    public class IllegalArgumentsInRootAction : IllegalArgumentsToChildActionTests
    {
        override protected string ThrowingAction { get { return "IllegalArgumentsInRoot"; } }
    }

    public class IllegalArgumentsInFirstChildAction : IllegalArgumentsToChildActionTests
    {
        override protected string ThrowingAction { get { return "IllegalArgumentsInFirstChild"; } }
    }

    public class IllegalArgumentsInNestedChildAction : IllegalArgumentsToChildActionTests
    {
        override protected string ThrowingAction { get { return "IllegalArgumentsInNestedChild"; } }
    }

}