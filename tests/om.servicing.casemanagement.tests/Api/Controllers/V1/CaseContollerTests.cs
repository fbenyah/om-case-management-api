using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using om.servicing.casemanagement.api.Controllers.V1;
using om.servicing.casemanagement.application.Features.OMCases.Queries;

namespace om.servicing.casemanagement.tests.Api.Controllers.V1;

public class CaseContollerTests
{    
    [Fact]
    public async Task GetCustomerCasesByIdentification_ReturnsOk_WhenResponseIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByIdentificationNumberResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByIdentificationNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByIdentification("source", "123");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByIdentification_ReturnsBadRequest_WhenResponseIsNotSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByIdentificationNumberResponse { };
        response.SetOrUpdateErrorMessage("Error occurred");

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByIdentificationNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByIdentification("source", "123");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(response, badRequestResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByIdentification_PassesCorrectQueryToMediator()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByIdentificationNumberResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByIdentificationNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        await controller.GetCustomerCasesByIdentification("source", "ABC123");

        // Assert
        mediatorMock.Verify(m => m.Send(
            It.Is<GetCustomerCasesByIdentificationNumberQuery>(q => q.IdentificationNumber == "ABC123"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private CaseContoller CreateControllerWithMediator(Mock<IMediator> mediatorMock)
    {
        var controller = new CaseContoller();

        // Setup Controller Context with mocked service provider
        var httpContext = new DefaultHttpContext();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IMediator))).Returns(mediatorMock.Object);
        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }
}
