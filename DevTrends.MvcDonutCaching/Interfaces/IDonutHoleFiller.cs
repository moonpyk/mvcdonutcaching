using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    public interface IDonutHoleFiller
    {
        /// <summary>
        /// Removes the donut hole wrappers.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <param name="options">The output cache options.</param>
        /// <returns></returns>
        string RemoveDonutHoleWrappers(string content, ControllerContext filterContext, OutputCacheOptions options);
        
        /// <summary>
        /// Replaces the donut holes content of with fresh content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <param name="options">The output cache options.</param>
        /// <returns></returns>
        string ReplaceDonutHoleContent(string content, ControllerContext filterContext, OutputCacheOptions options);
    }
}
