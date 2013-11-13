using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace DevTrends.MvcDonutCaching
{
    public class ControllerAction : IControllerAction
    {
        public readonly ActionSettings ActionSettings;
        public readonly ActionDescriptor ActionDescriptor;
        public readonly IDictionary<string, object> ActionParameters;

        public ControllerAction(ActionExecutingContext context)
        {
            Contract.Parameter.NotNull(context);
            ActionSettings = BuildActionSettings(context);
            ActionDescriptor = context.ActionDescriptor;
            ActionParameters = context.ActionParameters;
            if(ActionSettings == null || ActionDescriptor == null || ActionParameters == null)
            {
                throw new Exception("Something that should never be null was null");
            }
        }

        private static ActionSettings BuildActionSettings(ActionExecutingContext context)
        {
            Contract.Parameter.NotNull(context);
            return new ActionSettings()
                   {
                       ActionName = context.ActionDescriptor.ActionName,
                       ControllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName,
                       RouteValues = new RouteValueDictionary(context.ActionParameters)
                   };
        }

        public void Execute(ControllerContext context)
        {
            Contract.Parameter.NotNull(context);
            ActionDescriptor.Execute(context, ActionParameters);
        }
    }
}