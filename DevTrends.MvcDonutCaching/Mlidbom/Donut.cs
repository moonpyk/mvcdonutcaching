using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

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
        internal Donut Parent;
        private TextWriter _originalOutput;
        public ControllerAction ControllerAction { get; set; }
        public readonly List<string> OutputSegments = new List<string>();
        public List<Donut> Children = new List<Donut>();
        public bool Executed { get; set; }
        public readonly Guid Id = Guid.NewGuid();

        internal DonutOutputWriter Output { get; private set; }
        public bool Cached { get; set; }

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
        public string Execute(ActionExecutingContext context)
        {
            var output = new StringWriter();            

            for(int currentSegment = 0; currentSegment < OutputSegments.Count; currentSegment++)
            {
                output.Write(OutputSegments[currentSegment]);
                if(Children.Count > currentSegment)
                {
                    output.Write(InvokeChildAction(Children[currentSegment], context));
                }
            }

            if(Parent != null && !Parent.Executed)
            {
                return WrapInDonut(output.ToString());
            }
            else
            {
                return output.ToString();
            }
        }

        private string InvokeChildAction(Donut donut, ActionExecutingContext context)
        {
            ControllerBase controller = context.Controller;
            var viewContext = new ViewContext(
                controller.ControllerContext,
                new WebFormView(controller.ControllerContext, "tmp"),
                controller.ViewData,
                controller.TempData,
                TextWriter.Null
                );

            var htmlHelper = new HtmlHelper(viewContext, new ViewPage());

            return htmlHelper.Action(
                donut.ControllerAction.ActionDescriptor.ActionName,
                donut.ControllerAction.ActionDescriptor.ControllerDescriptor.ControllerName,
                new RouteValueDictionary( donut.ControllerAction.ActionParameters)).ToString();
        }

        public void ResultExecuted()
        {
            if(Executed)
            {
                if(Parent != null && !Parent.Executed)
                {
                    Parent.AddDonut(this);
                }
                return;
            }
            Executed = true;
            if(!ReferenceEquals(_context.HttpContext.Response.Output, Output))
            {
                throw new Exception("Hey! Someone replaced HttpContext.Response.Output and did not restore it correctly. Output will be corrupt.");
            }
            string totalOutput = ParseAndStoreOutput(Output.ToString());
            if(Parent != null && !Parent.Executed)
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
        private const string DonutHoleMiddle = "#18C1E8F8-B296-44BF-A768-89D4F41D14A6#";
        private const string DonutHoleEnd = "#18C1E8F8-B296-44BF-A768-89D4F41D14A6#EndDonut#";

        private static readonly string DonutCreationPattern = string.Format("{0}{{0}}{1}{{1}}{2}", DonutHoleStart, DonutHoleMiddle, DonutHoleEnd);
        private static readonly string DonutHolesPattern = string.Format("{0}(.*?){1}(.*?){2}", DonutHoleStart, DonutHoleMiddle,  DonutHoleEnd);

        private static readonly Regex DonutHolesRegexp = new Regex(DonutHolesPattern, RegexOptions.Compiled | RegexOptions.Singleline);
        private string ParseAndStoreOutput(string totalOutput)
        {
            if(OutputSegments.Any())
            {
                throw new InvalidOperationException("ParseAndStoreOutput should only ever be called once for a donut.");
            }
            var matches = DonutHolesRegexp.Matches(totalOutput);
            var sortedChildren = new List<Donut>();

            var currentIndex = 0;            
            var output = new StringWriter();
            foreach(Match match in matches)
            {
                var segment =  totalOutput.Substring(currentIndex, match.Index - currentIndex);
                currentIndex = match.Index + match.Length;
                var donutId = Guid.Parse(match.Groups[1].Value);
                sortedChildren.Add(Children.Single(child => child.Id == donutId));
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
            if(sortedChildren.Count != Children.Count)
            {
                throw new Exception("Oh the horror my child is lost!");
            }

            Children = sortedChildren;
            return output.ToString();
        }

        override public string ToString()
        {
            return string.Format("{0}.{1}(..) -> {2}",
                ControllerAction.ActionDescriptor.ControllerDescriptor,
                ControllerAction.ActionDescriptor.ActionName,
                OutputSegments.FirstOrDefault());
        }

        public void PushedFromCache(Donut parent)
        {
            Parent = parent;
            Cached = true;
        }        
    }
}