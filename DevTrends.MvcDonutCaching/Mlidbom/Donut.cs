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

        public Donut(Guid id, List<IDonut> sortedChildren, List<string> outputSegments, ControllerAction controllerAction)
        {
            _cached = false;
            Id = id;
            SortedChildren = sortedChildren;
            OutputSegments = outputSegments;
            ControllerAction = controllerAction;
        }

        public IDonut CloneWithNewParent(IDonutBuilder parent)
        {
            return new Donut(this, parent);
        }

        private Donut(IDonut source, IDonutBuilder parent):this(Guid.NewGuid(), source.SortedChildren, source.OutputSegments, source.ControllerAction)
        {
            _parent = parent;
            _cached = true;
        }

        public void ResultExecuted()
        {
            if(_parent != null)
            {
                _parent.AddChild(this);
            }
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
