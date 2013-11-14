using System;
using System.IO;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
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

        public static void ActionExecuting(ActionExecutingContext context)
        {
            Contract.Parameter.NotNull(context);
            var manager = context.HttpContext.Items[OutPutManagerId] as DonutOutputManager;
            if(manager == null)
            {
                context.HttpContext.Items[OutPutManagerId] = manager = new DonutOutputManager();
                manager._current = new Donut(context, null);                
            }
            else
            {
                var pushed = new Donut(context, manager._current);
                manager._current = pushed;
            }
            manager._depth++;
        }

        public static Donut ResultExecuted(ControllerContext context)
        {
            Contract.Parameter.NotNull(context);
            var manager = (DonutOutputManager)context.HttpContext.Items[OutPutManagerId];
            if(manager == null)
            {
                throw new InvalidOperationException("Attempt to pop when no donut has been pushed during this request.");
            }
            manager._depth--;
            var popped = manager._current;
            popped.ResultExecuted();
            if (manager._depth == 0)
            {
                manager._current = null;
                context.HttpContext.Items[OutPutManagerId] = null;
            }
            else
            {
                manager._current = popped.Parent;
            }            
            return popped;
        }
    }
}
