using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public abstract class AutoOutputCacheFilterProvider : IFilterProvider
    {
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var cacheAttributes = actionDescriptor.GetCustomAttributes(true)
                                    .OfType<AutoOutputCacheAttribute>()
                                    .ToArray();

            if(cacheAttributes.Count() == 1)
            {
                yield return new Filter(
                    CreateFilterInstance(cacheAttributes.Single()),
                    FilterScope.Action,
                    null);
            }

            if(cacheAttributes.Count() > 1)
            {
                throw new Exception("GetCustomAttributes returned more than one filter. Don't have any idea how to handle that..");
            }
        }

        ///<summary>
        /// <para>>Inheritors can override this method to provide the filter in any way they choose.</para>
        /// <para>Presumably various forms of DI will be common.</para>
        /// </summary>
        protected abstract AutoOutputCacheFilter CreateFilterInstance(AutoOutputCacheAttribute autoOutputCacheAttribute);
    }
}