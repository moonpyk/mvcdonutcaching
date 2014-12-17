using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Xunit;

namespace MvcDonutCaching.UnitTests
{
    public class ActionSettingsTests
    {
        [Fact]
        public void CorrectFieldsExist()
        {
            // Arrange
            const string actionName = "action";
            const string controllerName = "controller";
            var routeValueDictionary = new RouteValueDictionary();
            var actionSettings = new ActionSettings();

            // Act
            actionSettings.ActionName = actionName;
            actionSettings.ControllerName = controllerName;
            actionSettings.RouteValues = routeValueDictionary;

            // Assert
            Assert.Equal(actionName, actionSettings.ActionName);
            Assert.Equal(controllerName, actionSettings.ControllerName);
            Assert.Equal(routeValueDictionary, actionSettings.RouteValues);
        }
    }
}