using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ExcludeFromParentCacheAttribute : DonutOutputCacheAttribute
    {
        public ExcludeFromParentCacheAttribute() : this(new KeyBuilder()) { }

        public ExcludeFromParentCacheAttribute(IKeyBuilder keyBuilder) :
            base(
               new KeyGenerator(keyBuilder),
               new OutputCacheManager(OutputCache.Instance, keyBuilder),
               new DonutHoleFiller(new EncryptingActionSettingsSerialiser(new ActionSettingsSerialiser(), new Encryptor())),
               new CacheSettingsManager(),
               new CacheHeadersHelper()
        )
        { }

        override protected string WrapInDonutIfNeeded(ActionExecutingContext filterContext, string content)
        {
            var routeValues = new RouteValueDictionary(filterContext.ActionParameters);

            string serialisedActionSettings = HtmlHelperExtensions.GetSerialisedActionSettings(
                filterContext.ActionDescriptor.ActionName,
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                routeValues);

            return string.Format("<!--Donut#{0}#-->{1}<!--EndDonut-->", serialisedActionSettings, content);
        }
    }
}