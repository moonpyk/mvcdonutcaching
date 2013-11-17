using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    /// <summary>
    /// <para>>Minor convenience class that gives the advantage of easily telling if some other code has replaced our output by inspecting the type.</para>
    /// <para>It also gives us a debuggerDisplay that lets us figure out where this output is supposed to belong.</para>
    /// </summary>
    [DebuggerDisplay("{Description}")]
    public class DonutOutputWriter : StringWriter
    {
        public DonutOutputWriter(ActionDescriptor actionDescriptor):base(CultureInfo.InvariantCulture)
        {
            Description = string.Format("{0}.{1}", actionDescriptor.ActionName, actionDescriptor.ControllerDescriptor.ControllerName);
        }

        private string Description { get; set; }
    }
}