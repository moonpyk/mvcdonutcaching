using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Mlidbom;
using NUnit.Framework;

namespace MvcDonutCaching.Tests.Mlidbom
{
    public static class ControllerContextExtensions
    {
        public static ActionInvocationSimulator InvokeAction(
            this ActionExecutingContext context, 
            string controller = "controller", 
            string action = "action",
            string output = null,
            IDictionary<string, object> parameters = null)
        {
            var popper = new ActionInvocationSimulator(
                context, ctx => output,
                parameters ?? new Dictionary<string, object>(),
                controller, 
                action
                );            
            return popper;
        }
    }

    public class ActionInvocationSimulator : IDisposable
    {
        private readonly Func<ActionExecutingContext, string> _render;
        private readonly TextWriter _originalOutput;
        private ActionDescriptor ActionDescriptor { get; set; }
        private IDictionary<string, object> ActionParameters { get; set; }
        private ActionExecutingContext Context { get; set; }

        private Stack<ActionInvocationSimulator> PopperStack
        {
            get
            {
                var stack = (Stack<ActionInvocationSimulator>)Context.HttpContext.Items["ContextPopperStack"];
                if(Context.HttpContext.Items["ContextPopperStack"] == null)
                {
                    Context.HttpContext.Items["ContextPopperStack"] = stack = new Stack<ActionInvocationSimulator>();
                }
                return stack;
            }
        }

        public ActionInvocationSimulator(
            ActionExecutingContext context, 
            Func<ActionExecutingContext, string> render,
            IDictionary<string, object> parameters,
            string controller, 
            string action)
        {
            ActionParameters = context.ActionParameters = parameters ?? new Dictionary<string, object>();
            ActionDescriptor = context.ActionDescriptor = new StaticActionDescriptor(controllerName: controller, actionName: action);

  

            _render = render;
            Context = context;

            if(PopperStack.Count > 0)
            {
                var parent = PopperStack.Peek();
                parent.AddChild(this);
            }

            _originalOutput = context.HttpContext.Response.Output;
            context.HttpContext.Response.Output = new StringWriter();

            DonutOutputManager.ActionExecuting(context);
            PopperStack.Push(this);
        }

        private readonly List<ActionInvocationSimulator> _children = new List<ActionInvocationSimulator>();
        private string Output { get; set; }

        private void AddChild(ActionInvocationSimulator actionInvocationSimulator)
        {
            _children.Add(actionInvocationSimulator);
        }

        public void Dispose()
        {
            Output = string.Format(_render(Context), _children.Select(child => child.Output).ToArray());
            
            Context.HttpContext.Response.Output.Write(Output);//Simulate an action actually having executed and delivered output.

            DonutOutputManager.ResultExecuted(Context);

            var popped = PopperStack.Pop();
            Assert.That(popped, Is.SameAs(this), "Should be me....");

            if(PopperStack.Count > 0)//As far as we are concerned the output from child actions disappear into space as the code around us takes care of it
            {
                Context.HttpContext.Response.Output = _originalOutput;
            }else//Once we hit the root action we need to write the aggregate results to the response.
            {
                var whatTheDonutsDid = Context.HttpContext.Response.Output.ToString();
                Context.HttpContext.Response.Output = _originalOutput;
                Context.HttpContext.Response.Output.Write(whatTheDonutsDid);   
            }

            Context.ActionDescriptor = ActionDescriptor;//Restore the context.
            Context.ActionParameters = ActionParameters;            
        }

        
    }
}