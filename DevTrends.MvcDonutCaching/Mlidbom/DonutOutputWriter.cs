using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    /// <summary>
    /// Minor convenience class that gives the advantage of easily telling if some other code has replaced our output by inspecting the type...
    /// </summary>
    [DebuggerDisplay("{Description}: {ToString()}")]
    public class DonutOutputWriter : StringWriter
    {
        public DonutOutputWriter(ActionDescriptor actionDescriptor):base(CultureInfo.InvariantCulture)
        {
            Description = string.Format("{0}.{1}", actionDescriptor.ActionName, actionDescriptor.ControllerDescriptor.ControllerName);
        }

        private string Description { get; set; }
    }
}