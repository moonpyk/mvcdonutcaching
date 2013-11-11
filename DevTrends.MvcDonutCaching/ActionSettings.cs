using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public class ActionSettings
    {
        /// <summary>
        /// Gets or sets the action name.
        /// </summary>
        /// <value>
        /// The action's name.
        /// </value>
        public string ActionName { get; set; }
        
        /// <summary>
        /// Gets or sets the controller name.
        /// </summary>
        /// <value>
        /// The the controller name.
        /// </value>
        public string ControllerName { get; set; }
        
        /// <summary>
        /// Gets or sets the route values.
        /// </summary>
        /// <value>
        /// The route values.
        /// </value>
        public RouteValueDictionary RouteValues { get; set; }
    }
}