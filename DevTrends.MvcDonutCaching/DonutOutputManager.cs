using System;
using System.IO;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    /// <summary>
    /// Manages the respons output and the stack of Donuts as the request recurses down the action methods.
    /// </summary>
    public class DonutOutputManager
    {
        /// <summary>
        /// The currently executing Action filter with an effective attribute(On the action, controller, or parent controller)
        /// Actions with no effective attribute will be included in the output of their parent action.
        /// </summary>
        private Donut _current;

        private int _depth;

        private static readonly Guid OutPutManagerId = Guid.Parse("ADECE17F-38DD-420D-B7B8-AA15430A9920");
        internal TextWriter OriginalOutput;

        public static void Push(ActionExecutingContext context)
        {
            Contract.Parameter.NotNull(context);
            var manager = context.HttpContext.Items[OutPutManagerId] as DonutOutputManager;
            if(manager == null)
            {
                context.HttpContext.Items[OutPutManagerId] = manager = new DonutOutputManager();
                manager.OriginalOutput = context.HttpContext.Response.Output;
                manager._current = new Donut(context, null, manager.OriginalOutput);                
            }
            else
            {
                var pushed = new Donut(context, manager._current, manager.OriginalOutput);
                manager._current.AddDonut(pushed);
                manager._current = pushed;
            }
            manager._depth++;
            context.HttpContext.Response.Output = manager._current.Output;
        }

        public static Donut Pop(ControllerContext context)
        {
            Contract.Parameter.NotNull(context);
            var manager = (DonutOutputManager)context.HttpContext.Items[OutPutManagerId];
            if(manager == null)
            {
                throw new InvalidOperationException("Attempt to pop when no donut has been pushed during this request.");
            }
            manager._depth--;
            var popped = manager._current;
            popped.OnAfterExecute();
            if (manager._depth == 0)
            {
                manager._current = null;
                context.HttpContext.Items[OutPutManagerId] = null;
                context.HttpContext.Response.Output = manager.OriginalOutput;
            }
            else
            {
                manager._current = popped.Parent;
                context.HttpContext.Response.Output = manager._current.Output;
            }            
            return popped;
        }
    }
}
