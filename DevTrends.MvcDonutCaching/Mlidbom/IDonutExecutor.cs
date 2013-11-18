using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IDonutExecutor
    {       
        string Execute(ActionExecutingContext context);
    }
}