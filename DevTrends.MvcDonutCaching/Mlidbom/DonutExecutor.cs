using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class DonutExecutor : IDonutExecutor
    {
        private readonly IDonutBuilder _parent;
        private readonly IDonut _donut;
       
        public DonutExecutor(IDonut source, IDonutBuilder parent)
        {
            _donut = new Donut(source.ControllerAction, source.SortedChildren, source.SortedOutputSegments, true);
            _parent = parent;
        }

        override public string ToString()
        {
            return string.Format("{0} -> {1}", _donut.ControllerAction, _donut.SortedOutputSegments.FirstOrDefault());
        }

        public string Execute(ActionExecutingContext context)
        {
            var output = new StringWriter();
            var sortedOutputSegments = _donut.SortedOutputSegments.ToArray();
            var sortedChildren = _donut.SortedChildren.ToArray();
            for (int currentSegment = 0; currentSegment < sortedOutputSegments.Length; currentSegment++)
            {
                output.Write(sortedOutputSegments[currentSegment]);
                if (sortedChildren.Length > currentSegment)
                {
                    output.Write(sortedChildren[currentSegment].ControllerAction.Execute(context));
                }
            }

            if(_parent != null)
            {
                _parent.ChildResultExecuted(_donut);
                return _parent.PrepareChildOutput(_donut.Id, output.ToString());
            }
            return output.ToString();
        }
    }
}
