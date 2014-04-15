using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Demo.Areas.SubArea
{
    public class SubAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SubArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SubArea_default",
                "SubArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
