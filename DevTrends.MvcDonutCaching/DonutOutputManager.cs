using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public interface IControllerAction {
        void Execute(ControllerContext context);
    }

    public class ControllerAction : IControllerAction
    {
        public readonly ActionSettings ActionSettings;
        public readonly ActionDescriptor ActionDescriptor;
        public readonly IDictionary<string, object> ActionParameters;

        public ControllerAction(ActionExecutingContext context)
        {
            ActionSettings = BuildActionSettings(context);
            ActionDescriptor = context.ActionDescriptor;
            ActionParameters = context.ActionParameters;
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

        public void Execute(ControllerContext context)
        {
            ActionDescriptor.Execute(context, ActionParameters);
        }
    }
    public class Donut
    {
        internal readonly Donut Parent;
        private TextWriter _realOutput;
        public ControllerAction ControllerAction { get; set; }
        public readonly List<string> OutputList = new List<string>();
        public readonly List<IControllerAction> TrailingDonutList = new List<IControllerAction>();

        internal StringWriter Output { get; private set; }

        public Donut(ActionExecutingContext context, Donut parent, TextWriter realOutput)
        {
            ControllerAction = new ControllerAction(context);
            Parent = parent;
            _realOutput = realOutput;
            Output = new StringWriter(CultureInfo.InvariantCulture);
        }        

        internal void AddDonut(Donut child)
        {
            TrailingDonutList.Add(child.ControllerAction);
            FlushOutputSegment();
        }

        public void Execute(ActionExecutingContext context)
        {
            for(int currentOutputSegment = 0; currentOutputSegment < OutputList.Count -1; currentOutputSegment++)
            {
                context.HttpContext.Response.Write(OutputList[currentOutputSegment]);
                if(TrailingDonutList.Count > currentOutputSegment)
                {
                    TrailingDonutList[currentOutputSegment].Execute(context);
                }
            }
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

        public static void Push(ActionExecutingContext context)
        {
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
