using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IDonut : IDonutBuilder
    {
        Guid Id { get; }
        ControllerAction ControllerAction { get; }
        List<IDonut> SortedChildren { get; }
        List<string> OutputSegments { get; }
        bool Cached { get; }

        IDonut CloneWithNewParent(IDonutBuilder parent);

        string Execute(ActionExecutingContext context);
    }
}