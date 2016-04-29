﻿using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;

namespace DevTrends.MvcDonutCaching.Demo
{
    public class MvcApplication : HttpApplication
    {
        public IContainer Container
        {
            get;
            set;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Container = RegisterAutofac();

            this.SetSkipByCustomStringDelegate(SkipByCustomString);
        }

        private static IContainer RegisterAutofac()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<OutputCacheManager>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterFilterProvider();
            builder.RegisterModelBinderProvider();

            var container = builder.Build();
            container.ActivateGlimpse();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }
        
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (string.IsNullOrWhiteSpace(custom))
            {
                return base.GetVaryByCustomString(context, custom);
            }

            switch (custom.ToLowerInvariant())
            {
                case "subdomain":
                    return context.Request.Url.Host == "sub.localtest.me" 
                            ? "sub" 
                            : "main";


                case "user":
                    var principal = context.User;
                    if (principal != null)
                    {
                        return string.Format("{0}@{1}", principal.Identity.Name, principal.Identity.AuthenticationType);
                    }
                    break;
            }

            return base.GetVaryByCustomString(context, custom);
        }

        private static bool SkipByCustomString(HttpContextBase context, string custom)
        {
            switch (custom.ToLowerInvariant())
            {
                case "preview":
                    if (context.Request.QueryString["preview"] == "1")
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
