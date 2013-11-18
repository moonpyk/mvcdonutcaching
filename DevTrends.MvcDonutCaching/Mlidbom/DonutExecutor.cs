using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class DonutExecutor : IDonutExecutor
    {
        private readonly IDonutBuilder _parent;
        private readonly DonutOutputWriter _output;
        private readonly TextWriter _originalOutput;
        private readonly ActionExecutingContext _context;

        private readonly IDonut _donut;
       
        public DonutExecutor(IDonut source, IDonutBuilder parent, ActionExecutingContext context)
        {
            _donut = new Donut(Guid.NewGuid(), source.ControllerAction, source.SortedChildren, source.OutputSegments, true);
            _context = context;
            _output = new DonutOutputWriter(context.ActionDescriptor);
            _originalOutput = context.HttpContext.Response.Output;
            context.HttpContext.Response.Output = _output;
            _parent = parent;
        }

        public void ResultExecuted(bool wasException)
        {
            if (!ReferenceEquals(_context.HttpContext.Response.Output, _output))
            {
                throw new Exception("Hey! Someone replaced HttpContext.Response.Output and did not restore it correctly. Output will be corrupt.");
            }

            if(_parent != null)
            {
                _parent.AddChild(_donut);
                _originalOutput.Write(_parent.PrepareChildOutput(_donut.Id, _output.ToString()));
            }
            else
            {
                _originalOutput.Write(_output.ToString());
            }
            _context.HttpContext.Response.Output = _originalOutput;
        }

        public IDonut GetDonut()
        {
            return _donut;
        }

        public string PrepareChildOutput(Guid childId, string childOutput)
        {
            return childOutput;
        }

        override public string ToString()
        {
            return string.Format("{0} -> {1}", _donut.ControllerAction, _donut.OutputSegments.FirstOrDefault());
        }

        public string Execute(ActionExecutingContext context)
        {
            var output = new StringWriter();

            for (int currentSegment = 0; currentSegment < _donut.OutputSegments.Count; currentSegment++)
            {
                output.Write(_donut.OutputSegments[currentSegment]);
                if (_donut.SortedChildren.Count > currentSegment)
                {
                    output.Write(_donut.SortedChildren[currentSegment].ControllerAction.Execute(context));
                }
            }

            return output.ToString();
        }

        public void AddChild(IDonut child)
        {
        }
    }
}
