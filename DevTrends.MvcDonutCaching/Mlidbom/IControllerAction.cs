using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IControllerAction
    {
        void Execute(ControllerContext context);
    }
}
