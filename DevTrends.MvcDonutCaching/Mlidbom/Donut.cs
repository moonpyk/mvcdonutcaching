using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        public readonly Guid Id;

        internal DonutOutputWriter Output { get; private set; }

        public Donut(ActionExecutingContext context, Donut parent)
        {
            Id = Guid.NewGuid();
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
            string totalOutput = ParseAndStoreOutput(Output.ToString());
            if(Parent != null)
            {
                Parent.AddDonut(this);
                _originalOutput.Write(WrapInDonut(totalOutput));
            }
            else
            {
                _originalOutput.Write(totalOutput);
            }
            
            _context.HttpContext.Response.Output = _originalOutput;

            Output = null;            
        }

        private string WrapInDonut(string totalOutput)
        {
            string wrapInDonut = string.Format(DonutCreationPattern, Id, totalOutput);
            return wrapInDonut;
        }

        private const string DonutHoleStart = "#StartDonut#18C1E8F8-B296-44BF-A768-89D4F41D14A6#ID:";
        private const string DonutHoleSeparator = "#18C1E8F8-B296-44BF-A768-89D4F41D14A6#Value:";
        private const string DonutHoleEnd = "#18C1E8F8-B296-44BF-A768-89D4F41D14A6#EndDonut#";

        //todo:Remove. I used these to make it easier to see the structure during initial development/debugging.
        //private const string DonutHoleStart = "<Donut>";
        //private const string DonutHoleSeparator = "#Value:";
        //private const string DonutHoleEnd = "</Donut>";

        private static readonly string DonutCreationPattern = string.Format("{0}{{0}}{1}{{1}}{2}", DonutHoleStart, DonutHoleSeparator, DonutHoleEnd);
        private static readonly string DonutHolesPattern = string.Format("{0}(.*?){1}(.*?){2}", DonutHoleStart, DonutHoleSeparator, DonutHoleEnd);

        private static readonly Regex DonutHolesRegexp = new Regex(DonutHolesPattern, RegexOptions.Compiled | RegexOptions.Singleline);
        private string ParseAndStoreOutput(string totalOutput)
        {            
            var matches = DonutHolesRegexp.Matches(totalOutput);

            var currentIndex = 0;            
            var output = new StringWriter();
            foreach(Match match in matches)
            {
                var segment =  totalOutput.Substring(currentIndex, match.Index - currentIndex);
                currentIndex = match.Index + match.Length;
                OutputSegments.Add(segment);
                output.Write(segment);
                output.Write(match.Groups[2].Value);
            }

            if(currentIndex < totalOutput.Length)
            {
                var segment = totalOutput.Substring(currentIndex);
                OutputSegments.Add(segment);
                output.Write(segment);
            }

            return output.ToString();
        }
    }
}