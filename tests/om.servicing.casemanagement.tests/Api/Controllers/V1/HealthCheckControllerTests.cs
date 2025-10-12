using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using om.servicing.casemanagement.api.Controllers.V1;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Api.Controllers.V1;

public class HealthCheckControllerTests
{
    private HealthCheckController CreateControllerWithLoggingService(Mock<ILoggingService> loggingServiceMock)
    {
        var controller = new HealthCheckController();

        // Setup Controller Context with mocked service provider
        var httpContext = new DefaultHttpContext();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILoggingService))).Returns(loggingServiceMock.Object);
        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }

    [Fact]
    public void Get_ReturnsOkWithMessage_AndLogsInfo()
    {
        // Arrange
        var loggingServiceMock = new Mock<ILoggingService>();
        var controller = CreateControllerWithLoggingService(loggingServiceMock);

        // Act
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var message = Assert.IsType<string>(okResult.Value);
        Assert.Contains("CASE MANAGEMENT API is running", message);

        loggingServiceMock.Verify(x => x.LogInfo(It.Is<string>(s => s.Contains("CASE MANAGEMENT API is running"))), Times.Once);
    }
}
