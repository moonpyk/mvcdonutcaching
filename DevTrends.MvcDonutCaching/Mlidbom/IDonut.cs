using System;
using System.Collections.Generic;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IDonut
    {
        Guid Id { get; }
        ControllerAction ControllerAction { get; }
        IEnumerable<IDonut> SortedChildren { get; }
        IEnumerable<string> SortedOutputSegments { get; }
        Donut CloneForCache();
        bool Cached { get; } //todo: not happy about having this here. Try to get rid of it.
    }
}