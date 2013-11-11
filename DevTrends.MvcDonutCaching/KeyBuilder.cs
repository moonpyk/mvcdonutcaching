using System.Collections.Generic;
using System.Text;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public class KeyBuilder : IKeyBuilder
    {
        private string _cacheKeyPrefix = "_d0nutc@che.";

        public string CacheKeyPrefix
        {
            get
            {
                return _cacheKeyPrefix;
            }
            set
            {
                _cacheKeyPrefix = value;
            }
        }

        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        public string BuildKey(string controllerName)
        {
            return BuildKey(controllerName, null, null);
        }

        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        public string BuildKey(string controllerName, string actionName)
        {
            return BuildKey(controllerName, actionName, null);
        }

        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="routeValues">The route values.</param>
        public string BuildKey(string controllerName, string actionName, RouteValueDictionary routeValues)
        {
            var builder = new StringBuilder(CacheKeyPrefix);

            if (controllerName != null)
            {
                builder.AppendFormat("{0}.", controllerName.ToLowerInvariant());
            }

            if (actionName != null)
            {
                builder.AppendFormat("{0}#", actionName.ToLowerInvariant());
            }

            if (routeValues != null)
            {
                foreach (var routeValue in routeValues)
                {
                    builder.Append(BuildKeyFragment(routeValue));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Builds the cache key fragment.
        /// </summary>
        /// <param name="routeValue">The route value to process.</param>
        public string BuildKeyFragment(KeyValuePair<string, object> routeValue)
        {
            var value = routeValue.Value == null ? "<null>" : routeValue.Value.ToString().ToLowerInvariant();

            return string.Format("{0}={1}#", routeValue.Key.ToLowerInvariant(), value);
        }
    }
}
