using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public static class DonutExecutor
    {
        public static string ExecuteResult(this IDonut donut, IDonutBuilder parent, ActionExecutingContext context)
        {
            var output = new StringWriter();
            var sortedOutputSegments = donut.SortedOutputSegments.ToArray();
            var sortedChildren = donut.SortedChildren.ToArray();
            for (int currentSegment = 0; currentSegment < sortedOutputSegments.Length; currentSegment++)
            {
                output.Write(sortedOutputSegments[currentSegment]);
                if (sortedChildren.Length > currentSegment)
                {
                    output.Write(sortedChildren[currentSegment].ControllerAction.Execute(context));
                }
            }

            if (parent != null)
            {
                parent.ChildResultExecuted(donut);
                return parent.PrepareChildOutput(donut.Id, output.ToString());
            }
            return output.ToString();
        }
    }
}
