using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class DonutBuilder : IDonutBuilder
    {
        private readonly ActionExecutingContext _context;
        private readonly TextWriter _originalOutput;
        private readonly Dictionary<Guid, IDonut> _children = new Dictionary<Guid, IDonut>();
        private readonly IDonutBuilder _parent;
        private readonly DonutOutputWriter _output;
        private readonly Guid _id;  

        private Donut ExecutedVersion { get; set; }
        public IDonut CacheAbleValue()
        {
            return ExecutedVersion;
        }

        public string PrepareChildOutput(Guid childId, string childOutput)
        {
            return DonutHole.WrapInDonut(childId, childOutput);
        }

        public DonutBuilder(ActionExecutingContext context, IDonutBuilder parent)
        {
            Contract.Parameter.NotNull(context);
            _id = Guid.NewGuid();            
            _context = context;
            _parent = parent;
            _originalOutput = _context.HttpContext.Response.Output;
            _output = new DonutOutputWriter(context.ActionDescriptor);
            context.HttpContext.Response.Output = _output;
        }

        public void AddChild(IDonut child)
        {
            Contract.Parameter.NotNull(child);
            _children[child.Id] = child;
        }

        public void ResultExecuted(bool wasException)
        {
            if(ExecutedVersion != null)
            {
                throw new InvalidOperationException("Already executed.");
            }
            if(!ReferenceEquals(_context.HttpContext.Response.Output, _output))
            {
                throw new Exception("Hey! Someone replaced HttpContext.Response.Output and did not restore it correctly. Output will be corrupt.");
            }

            if(wasException)//We don't mess with the output if an exception was thrown. Nothing will be cached and we do not want to break things worse.
            {
                _originalOutput.Write(_output.ToString());
            }
            else
            {
                var totalOutput = ParseAndStoreOutput(_output.ToString());
                if(_parent != null)
                {
                    _parent.AddChild(CacheAbleValue());
                    _originalOutput.Write(_parent.PrepareChildOutput(_id, totalOutput));
                }
                else
                {
                    _originalOutput.Write(totalOutput);
                }
            }

            _context.HttpContext.Response.Output = _originalOutput;
        }

        private string ParseAndStoreOutput(string totalOutput)
        {
            var matches = DonutHole.FindDonuts(totalOutput);
            var sortedChildren = new List<IDonut>();
            var outputSegments = new List<string>();

            var currentIndex = 0;            
            var output = new StringWriter();
            foreach(Match match in matches)
            {
                var segment =  totalOutput.Substring(currentIndex, match.Index - currentIndex);
                currentIndex = match.Index + match.Length;
                var donutId = Guid.Parse(match.Groups[1].Value);
                sortedChildren.Add(_children[donutId]);
                outputSegments.Add(segment);
                output.Write(segment);
                output.Write(match.Groups[2].Value);
            }

            if(currentIndex < totalOutput.Length)
            {
                var segment = totalOutput.Substring(currentIndex);
                outputSegments.Add(segment);
                output.Write(segment);
            }

            if(_children.Count != sortedChildren.Count)
            {
                throw new Exception("Children have gone missing.");
            }

            ExecutedVersion = new Donut(_id, sortedChildren, outputSegments, new ControllerAction(_context));

            return output.ToString();
        }

        override public string ToString()
        {
            return string.Format("{0}.{1}(..) -> {2}",
                _context.ActionDescriptor.ControllerDescriptor.ControllerName,
                _context.ActionDescriptor.ActionName,
                _output);
        }
    }
}