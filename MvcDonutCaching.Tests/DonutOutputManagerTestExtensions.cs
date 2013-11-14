using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace MvcDonutCaching.Tests
{
    public static class DonutOutputManagerTestExtensions
    {
        public static ContextPopper FakeControllerInvocationAndPushDonut(
            this ActionExecutingContext context, 
            string controller = "controller", 
            string action = "action",
            string output = null,
            IDictionary<string, object> parameters = null)
        {
            var popper = new ContextPopper(
                context, ctx => output,
                parameters ?? new Dictionary<string, object>(),
                controller, 
                action
                );            
            return popper;
        }
    }

    public class ContextPopper : IDisposable
    {
        private readonly Func<ActionExecutingContext, string> _render;
        private TextWriter _originalOutput;
        private ActionDescriptor ActionDescriptor { get; set; }
        private IDictionary<string, object> ActionParameters { get; set; }
        private ActionExecutingContext Context { get; set; }

        private Stack<ContextPopper> PopperStack
        {
            get
            {
                var stack = (Stack<ContextPopper>)Context.HttpContext.Items["ContextPopperStack"];
                if(Context.HttpContext.Items["ContextPopperStack"] == null)
                {
                    Context.HttpContext.Items["ContextPopperStack"] = stack = new Stack<ContextPopper>();
                }
                return stack;
            }
        }

        public ContextPopper(
            ActionExecutingContext context, 
            Func<ActionExecutingContext, string> render,
            IDictionary<string, object> parameters,
            string controller, 
            string action)
        {
            ActionParameters = context.ActionParameters = parameters ?? new Dictionary<string, object>();
            ActionDescriptor = context.ActionDescriptor = new StaticActionDescriptor(controllerName: controller, actionName: action);

            DonutOutputManager.Push(context);

            _render = render;
            Context = context;

            if(PopperStack.Count > 0)
            {
                var parent = PopperStack.Peek();
                parent.AddChild(this);
            }
            PopperStack.Push(this);
        }

        public List<ContextPopper> Children = new List<ContextPopper>();
        public string Output { get; set; }

        private void AddChild(ContextPopper contextPopper)
        {
            Children.Add(contextPopper);
        }

        public void Dispose()
        {
            Output = string.Format(_render(Context), Children.Select(child => child.Output).ToArray());
            Context.HttpContext.Response.Output.Write(Output);
            DonutOutputManager.Pop(Context);
            Context.ActionDescriptor = ActionDescriptor;
            Context.ActionParameters = ActionParameters;            
        }

        
    }
}