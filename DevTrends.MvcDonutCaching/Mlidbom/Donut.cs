using System;
using System.Collections.Generic;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class Donut : IDonut
    {
        public Guid Id { get; private set; }
        public ControllerAction ControllerAction { get; private set; }
        public List<IDonut> SortedChildren { get; private set; }
        public List<string> OutputSegments { get; private set; }
        public bool Cached { get; private set; }

        public Donut(Guid id, ControllerAction controllerAction, List<IDonut> sortedChildren, List<string> outputSegments, bool cached)
        {
            Id = id;
            ControllerAction = controllerAction;
            SortedChildren = sortedChildren;
            OutputSegments = outputSegments;
            Cached = cached;
        }

        public Donut(IDonut source):this(source.Id, source.ControllerAction, source.SortedChildren, source.OutputSegments, source.Cached)
        {
            
        }
    }
}