using System;
using System.Collections.Generic;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class Donut : IDonut
    {
        public Guid Id { get; private set; }
        public bool Cached { get; private set; }
        public ControllerAction ControllerAction { get; private set; }
        public IEnumerable<IDonut> SortedChildren { get; private set; }
        public IEnumerable<string> SortedOutputSegments { get; private set; }
        
        public Donut CloneForCache()
        {
            return new Donut(this, cached:true);
        }        

        private Donut(IDonut source, bool cached) : this(source.ControllerAction, source.SortedChildren, source.SortedOutputSegments)
        {
            Id = source.Id;
            Cached = true;
        }

        public Donut(ControllerAction controllerAction, IEnumerable<IDonut> sortedChildren, IEnumerable<string> sortedOutputSegments)
        {
            Id = Guid.NewGuid();
            ControllerAction = controllerAction;
            SortedChildren = sortedChildren;
            SortedOutputSegments = sortedOutputSegments;
            Cached = false;
        }
    }
}