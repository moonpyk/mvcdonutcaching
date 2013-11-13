using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DevTrends.MvcDonutCaching
{
    /// <summary>
    /// Keeps track of the output of executing a controller action.
    /// 
    /// <para> Stores output specifically created by this action in <see cref="OutputSegments"/></para>
    /// <para>Track of which child actions can fill the holes in <see cref="OutputSegments"/> in <see cref="ChildActions"/>.</para>
    /// <para>Call <see cref="Execute"/> to output the cached data and recurse down to child actions to fill in the blanks.</para>
    /// </summary>
    public class Donut
    {
        internal readonly Donut Parent;
        private TextWriter _realOutput;
        public ControllerAction ControllerAction { get; set; }
        public readonly List<string> OutputSegments = new List<string>();
        public readonly List<IControllerAction> ChildActions = new List<IControllerAction>();

        internal StringWriter Output { get; private set; }

        public Donut(ActionExecutingContext context, Donut parent, TextWriter realOutput)
        {
            Contract.Parameter.NotNull(context, realOutput);
            ControllerAction = new ControllerAction(context);
            Parent = parent;
            _realOutput = realOutput;
            Output = new StringWriter(CultureInfo.InvariantCulture);
        }

        internal void AddDonut(Donut child)
        {
            Contract.Parameter.NotNull(child);
            ChildActions.Add(child.ControllerAction);
            FlushOutputSegment();
        }

        /// <summary>
        /// Writes the stored output to the response stream and recurses down to <see cref="ChildActions"/> to fill in the holes.
        /// </summary>
        /// <param name="context">Execute </param>
        public void Execute(ActionExecutingContext context)
        {            
            Contract.Parameter.NotNull(context);
            for(int currentOutputSegment = 0; currentOutputSegment < OutputSegments.Count; currentOutputSegment++)
            {
                context.HttpContext.Response.Write(OutputSegments[currentOutputSegment]);
                if(ChildActions.Count > currentOutputSegment)
                {
                    ChildActions[currentOutputSegment].Execute(context);
                }
            }
        }

        private void FlushOutputSegment()
        {
            var outputSegment = Output.ToString();
            Output = new StringWriter(CultureInfo.InvariantCulture);
            OutputSegments.Add(outputSegment);            
            _realOutput.Write(outputSegment);
        }

        public void OnAfterExecute()
        {
            FlushOutputSegment();
            Output = null;
            _realOutput = null;
        }        
    }
}