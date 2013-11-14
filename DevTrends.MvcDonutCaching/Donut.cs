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
    /// <para>Track of which child actions can fill the holes in <see cref="OutputSegments"/> in <see cref="Children"/>.</para>
    /// <para>Call <see cref="Execute"/> to output the cached data and recurse down to child actions to fill in the blanks.</para>
    /// </summary>
    public class Donut
    {
        private readonly ActionExecutingContext _context;
        internal readonly Donut Parent;
        private TextWriter _originalOutput;
        public ControllerAction ControllerAction { get; set; }
        public readonly List<string> OutputSegments = new List<string>();
        public readonly List<Donut> Children = new List<Donut>();

        internal StringWriter Output { get; private set; }

        public Donut(ActionExecutingContext context, Donut parent)
        {
            Contract.Parameter.NotNull(context);
            ControllerAction = new ControllerAction(context);
            _context = context;
            Parent = parent;
            _originalOutput = _context.HttpContext.Response.Output;
            Output = new StringWriter(CultureInfo.InvariantCulture);
            context.HttpContext.Response.Output = Output;
        }

        internal void AddDonut(Donut child)
        {
            Contract.Parameter.NotNull(child);
            Children.Add(child);
        }

        /// <summary>
        /// Writes the stored output to the response stream and recurses down to <see cref="Children"/> to fill in the holes.
        /// </summary>
        /// <param name="context">Execute </param>
        public void Execute(ActionExecutingContext context)
        {            
            Contract.Parameter.NotNull(context);
            for(int currentOutputSegment = 0; currentOutputSegment < OutputSegments.Count; currentOutputSegment++)
            {
                context.HttpContext.Response.Write(OutputSegments[currentOutputSegment]);
                if(Children.Count > currentOutputSegment)
                {
                    Children[currentOutputSegment].Execute(context);
                }
            }
        }

        public void ResultExecuted()
        {
            if(Parent != null)
            {
                Parent.AddDonut(this);
                _originalOutput.Write("<NUT>{0}</NUT>", Output.ToString());
            }
            else
            {
                //_originalOutput.Write(Output.ToString());
            }
            Console.WriteLine("ResultExecuted1: _originalOutput: '{0}', Output: '{1}'", _originalOutput, Output);
            
            _context.HttpContext.Response.Output = _originalOutput;

            Output = null;            
        }        
    }
}