using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public class Donut
    {
        internal readonly Donut Parent;
        private TextWriter _realOutput;
        public readonly ActionSettings ActionSettings;
        public readonly ActionDescriptor ActionDescriptor;
        public readonly IDictionary<string, object> ActionParameters; 
        public readonly string CacheKey;
        public readonly List<string> OutputList = new List<string>();
        public readonly List<string> TrailingDonutList = new List<string>();

        internal StringWriter Output { get; private set; }

        public Donut(ActionExecutingContext context, Donut parent, TextWriter realOutput, ActionSettings actionSettings, string cacheKey)
        {
            ActionSettings = BuildActionSettings(context);
            ActionDescriptor = context.ActionDescriptor;
            ActionParameters = context.ActionParameters;
            Parent = parent;
            _realOutput = realOutput;
            ActionSettings = actionSettings;
            CacheKey = cacheKey;
            Output = new StringWriter(CultureInfo.InvariantCulture);
        }

        internal void AddDonut(string child)
        {
            TrailingDonutList.Add(child);
            FlushOutputSegment();
        }

        private void FlushOutputSegment()
        {
            var outputSegment = Output.ToString();
            Output = new StringWriter(CultureInfo.InvariantCulture);
            OutputList.Add(outputSegment);            
            _realOutput.Write(outputSegment);
        }

        public void OnAfterExecute()
        {
            FlushOutputSegment();
            Output = null;
            _realOutput = null;
        }


        private static ActionSettings BuildActionSettings(ActionExecutingContext context)
        {
            return new ActionSettings()
            {
                ActionName = context.ActionDescriptor.ActionName,
                ControllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName,
                RouteValues = new RouteValueDictionary(context.ActionParameters)
            };
        }
    }

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

        public static void Push(ActionExecutingContext context, string cacheKey)
        {
            var manager = context.HttpContext.Items[OutPutManagerId] as DonutOutputManager;
            if(manager == null)
            {
                context.HttpContext.Items[OutPutManagerId] = manager = new DonutOutputManager();
                manager.OriginalOutput = context.HttpContext.Response.Output;
                manager._current = new Donut(context, null, manager.OriginalOutput, BuildActionSettings(context), cacheKey);                
            }
            else
            {
                var pushed = new Donut(context, manager._current, manager.OriginalOutput, BuildActionSettings(context), cacheKey);
                manager._current.AddDonut(pushed.CacheKey);
                manager._current = pushed;
            }
            manager._depth++;
            context.HttpContext.Response.Output = manager._current.Output;
        }

        private static ActionSettings BuildActionSettings(ActionExecutingContext context)
        {
            return new ActionSettings()
                   {
                       ActionName = context.ActionDescriptor.ActionName,
                       ControllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName,
                       RouteValues = new RouteValueDictionary(context.ActionParameters)
                   };
        }

        public static Donut Pop(ControllerContext context)
        {
            var manager = (DonutOutputManager)context.HttpContext.Items[OutPutManagerId];
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
