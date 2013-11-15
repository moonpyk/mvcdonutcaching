using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class DonutOutputManager
    {
        private static readonly Guid DonutStackId = Guid.Parse("BBD4587E-4905-43CB-9BEC-6B68D3704F4E");

        private static Stack<Donut> DonutStack(ControllerContext context)
        {
            Contract.Parameter.NotNull(context); 
            var donutStack = context.HttpContext.Items[DonutStackId] as Stack<Donut>;
            if (donutStack == null)
            {
                context.HttpContext.Items[DonutStackId] = donutStack = new Stack<Donut>();
            }
            return donutStack;
        }

        public static void ActionExecuting(ActionExecutingContext context)
        {
            var donutStack = DonutStack(context);
            var pushed = new Donut(context, donutStack.Count > 0 ? donutStack.Peek() : null);
            donutStack.Push(pushed);
        }

        public static Donut ResultExecuted(ControllerContext context)
        {            
            var executed = DonutStack(context).Pop();;
            executed.ResultExecuted();
            return executed;
        }
    }
}
