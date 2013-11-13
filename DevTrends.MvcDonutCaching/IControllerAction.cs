using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching
{
    public interface IControllerAction {
        void Execute(ControllerContext context);
    }
}