using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public class ControllerAction
    {
        public readonly string ActionName;        
        public readonly string ControllerName;
        public readonly IDictionary<string, object> ActionParameters;

        public ControllerAction(ActionExecutingContext context)
        {
            ActionName = context.ActionDescriptor.ActionName;
            ControllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;

            Contract.Parameter.NotNull(context);
            ActionParameters =  context.ActionParameters;

            if(ActionName == null || ControllerName == null || ActionParameters == null)
            {
                throw new Exception("Something that should never be null was null");
            }
        }

        override public string ToString()
        {
            return string.Format("{0}.{1}(..)", ControllerName, ActionName);
        }
    }
}