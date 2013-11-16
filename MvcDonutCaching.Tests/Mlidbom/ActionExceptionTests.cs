using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    [TestFixture]
    public abstract class ActionExceptionTests : TestsBase
    {
        private const string ThrownExceptionText = "ExceptionTextToFindInOutput";
        private const string Controller = "ThrowException";
        protected abstract string ThrowingAction { get; }

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
            return GetUrlContent(string.Format("/{0}/{1}", Controller, ThrowingAction));
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