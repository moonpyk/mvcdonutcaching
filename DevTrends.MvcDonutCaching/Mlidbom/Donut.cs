using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
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

        internal DonutOutputWriter Output { get; private set; }

        public Donut(ActionExecutingContext context, Donut parent)
        {
            Contract.Parameter.NotNull(context);
            ControllerAction = new ControllerAction(context);
            _context = context;
            Parent = parent;
            _originalOutput = _context.HttpContext.Response.Output;
            Output = new DonutOutputWriter(context.ActionDescriptor);
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
            if(!ReferenceEquals(_context.HttpContext.Response.Output, Output))
            {
                throw new Exception("Hey! Someone replaced HttpContext.Response.Output and did not restore it correctly. Output will be corrupt.");
            }
            string totalOutput = Output.ToString();
            ParseAndStoreOutput(totalOutput);
            if(Parent != null)
            {
                Parent.AddDonut(this);
                _originalOutput.Write(totalOutput);
            }
            else
            {
                _originalOutput.Write(totalOutput);
            }
            
            _context.HttpContext.Response.Output = _originalOutput;

            Output = null;            
        }

        private void ParseAndStoreOutput(string totalOutput)
        {
            OutputSegments.Add(totalOutput);
        }
    }
}