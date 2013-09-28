using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;

namespace DevTrends.MvcDonutCaching.Demo
{
    public class MvcApplication : System.Web.HttpApplication
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
        }

        private static IContainer RegisterAutofac()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<OutputCacheManager>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerHttpRequest();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterFilterProvider();
            builder.RegisterModelBinderProvider();

            var container = builder.Build();
            container.ActivateGlimpse();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }
    }
}