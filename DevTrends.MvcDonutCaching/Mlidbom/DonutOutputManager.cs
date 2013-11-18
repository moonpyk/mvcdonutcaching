using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public static class DonutOutputManager
    {
        private static readonly Guid DonutStackId = Guid.Parse("BBD4587E-4905-43CB-9BEC-6B68D3704F4E");

        private static Stack<IDonutBuilder> DonutStack(ControllerContext context)
        {
            Contract.Parameter.NotNull(context);
            var donutStack = context.HttpContext.Items[DonutStackId] as Stack<IDonutBuilder>;
            if (donutStack == null)
            {
                context.HttpContext.Items[DonutStackId] = donutStack = new Stack<IDonutBuilder>();
            }
            return donutStack;
        }

        public static IDonutExecutor ActionExecutingCached(ActionExecutingContext context, IDonut cached)
        {
            var donutStack = DonutStack(context);
            var parent = donutStack.Count > 0 ? donutStack.Peek() : null;
            var donutExecutor = new DonutExecutor(cached, parent);
            donutStack.Push(donutExecutor);
            return donutExecutor;
        } 

        public static void ActionExecuting(ActionExecutingContext context)
        {
            var donutStack = DonutStack(context);
            var parent = donutStack.Count > 0 ? donutStack.Peek() : null;
            var pushed = new DonutBuilder(context, parent);
            donutStack.Push(pushed);
        }

        public static void ResultExecutionFailed(ControllerContext context)
        {
            ResultExecuted(context, wasException: true);
        }

        public static IDonut ResultExecutionSucceeded(ControllerContext context)
        {
            return ResultExecuted(context, wasException: false);
        }

        private static IDonut ResultExecuted(ControllerContext context, bool wasException)
        {            
            var executed = DonutStack(context).Pop();;
            executed.ResultExecuted(wasException);
            return executed.GetDonut();
        }
    }
}
