using System;
using System.Net;
using NCrunch.Framework;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture, ExclusivelyUses(Controller)]
    public abstract class ActionExceptionTests : ControllerTestBase
    {
        private const string ThrownExceptionText = "ExceptionTextToFindInOutput";
        public const string Controller = "ThrowException";
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
            Assert.That(thrown, Is.Not.Null);
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

    public class ExceptionThrownInRootAction : ActionExceptionTests
    {
        override protected string ThrowingAction { get { return "ThrowNow"; } }
    }

    public class ExceptionThrownInFirstChildAction : ActionExceptionTests
    {
        override protected string ThrowingAction { get { return "ThrowInNextLevel"; } }
    }

    public class ExceptionThrownInNestedChildAction : ActionExceptionTests
    {
        override protected string ThrowingAction { get { return "ThrowTwoLevelsDown"; } }
    }

}