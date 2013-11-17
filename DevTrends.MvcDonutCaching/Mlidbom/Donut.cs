using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class Donut : IDonut
    {
        public List<string> OutputSegments { get; private set; }
        public List<IDonut> SortedChildren { get; private set; }
        public ControllerAction ControllerAction { get; private set; }
        public Guid Id { get; private set; }
        private readonly IDonutBuilder _parent;
        private readonly bool _cached;
        public bool Cached { get { return _cached; } }
        private DonutOutputWriter _output;
        private TextWriter _originalOutput;
        private ActionExecutingContext _context;

        public Donut(Guid id, List<IDonut> sortedChildren, List<string> outputSegments, ControllerAction controllerAction)
        {
            _cached = false;
            Id = id;
            SortedChildren = sortedChildren;
            OutputSegments = outputSegments;
            ControllerAction = controllerAction;
        }

        public IDonut CloneWithNewParent(IDonutBuilder parent, ActionExecutingContext context)
        {
            return new Donut(this, parent, context);
        }

        private Donut(IDonut source, IDonutBuilder parent, ActionExecutingContext context):this(Guid.NewGuid(), source.SortedChildren, source.OutputSegments, source.ControllerAction)
        {
            _context = context;
            _output = new DonutOutputWriter(context.ActionDescriptor);
            _originalOutput = context.HttpContext.Response.Output;
            context.HttpContext.Response.Output = _output;
            _parent = parent;
            _cached = true;
        }

        public void ResultExecuted()
        {
            if (!ReferenceEquals(_context.HttpContext.Response.Output, _output))
            {
                throw new Exception("Hey! Someone replaced HttpContext.Response.Output and did not restore it correctly. Output will be corrupt.");
            }

            if(_parent != null)
            {
                _parent.AddChild(this);
                _originalOutput.Write(_parent.PrepareChildOutput(Id, _output.ToString()));
            }
            else
            {
                _originalOutput.Write(_output.ToString());
            }
            _context.HttpContext.Response.Output = _originalOutput;
        }

        public IDonut CacheAbleValue()
        {
            return this;
        }

        public string PrepareChildOutput(Guid childId, string childOutput)
        {
            return childOutput;
        }

        override public string ToString()
        {
            return string.Format("{0} -> {1}", ControllerAction, OutputSegments.FirstOrDefault());
        }

        public string Execute(ActionExecutingContext context)
        {
            var output = new StringWriter();

            for(int currentSegment = 0; currentSegment < OutputSegments.Count; currentSegment++)
            {
                output.Write(OutputSegments[currentSegment]);
                if(SortedChildren.Count > currentSegment)
                {
                    output.Write(SortedChildren[currentSegment].ControllerAction.Execute(context));
                }
            }

            if(_parent != null)
            {
                return _parent.PrepareChildOutput(Id, output.ToString());
            }
            return output.ToString();
        }

        public void AddChild(IDonut child)
        {
        }
    }
}
