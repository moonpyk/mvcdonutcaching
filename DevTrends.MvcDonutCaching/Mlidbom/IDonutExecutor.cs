using System.Web.Mvc;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IDonutExecutor : IDonutBuilder
    {       
        string Execute(ActionExecutingContext context);
    }
}