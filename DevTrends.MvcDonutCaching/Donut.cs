using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DevTrends.MvcDonutCaching
{
    public class Donut
    {
        internal readonly Donut Parent;
        private TextWriter _realOutput;
        public ControllerAction ControllerAction { get; set; }
        public readonly List<string> OutputList = new List<string>();
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

        public void Execute(ActionExecutingContext context)
        {            
            Contract.Parameter.NotNull(context);
            for(int currentOutputSegment = 0; currentOutputSegment < OutputList.Count -1; currentOutputSegment++)
            {
                context.HttpContext.Response.Write(OutputList[currentOutputSegment]);
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
}