using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using om.servicing.casemanagement.api.Controllers.V1;
using om.servicing.casemanagement.application.Features.OMCases.Commands;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.application.Services.Models;

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

    [Fact]
    public async Task GetCustomerCasesByIdentificationAndStatus_ReturnsOk_WhenResponseIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByIdentificationNumberAndStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByIdentificationAndStatus("source", "123", "Open");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByIdentificationAndStatus_ReturnsBadRequest_WhenResponseIsNotSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse { };
        response.SetOrUpdateErrorMessage("Error occurred");

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByIdentificationNumberAndStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByIdentificationAndStatus("source", "123", "Open");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(response, badRequestResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByIdentificationAndStatus_PassesCorrectQueryToMediator()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByIdentificationNumberAndStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        await controller.GetCustomerCasesByIdentificationAndStatus("source", "ABC123", "Closed");

        // Assert
        mediatorMock.Verify(m => m.Send(
            It.Is<GetCustomerCasesByIdentificationNumberAndStatusQuery>(q => q.IdentificationNumber == "ABC123" && q.Status == "Closed"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateShellCase_ReturnsOk_WhenSuccess()
    {
        var response = new CreateShellCaseCommandResponse
        {
            Data = new BasicCaseCreateResponse { Id = "CASE123", ReferenceNumber = "REF456" }
        };

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateShellCaseCommand>(), default))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);
        var result = await controller.CreateShellCase("TestSourceSystem");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<CreateShellCaseCommandResponse>(okResult.Value);
        Assert.True(returnedResponse.Success);
        Assert.Equal("CASE123", returnedResponse.Data.Id);
        Assert.Equal("REF456", returnedResponse.Data.ReferenceNumber);
    }

    [Fact]
    public async Task CreateShellCase_ReturnsBadRequest_WhenNotSuccess()
    {
        var response = new CreateShellCaseCommandResponse
        {
        };
        response.SetOrUpdateErrorMessage("Failed to create shell case.");

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateShellCaseCommand>(), default))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);
        var result = await controller.CreateShellCase("TestSourceSystem");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedResponse = Assert.IsType<CreateShellCaseCommandResponse>(badRequestResult.Value);
        Assert.False(returnedResponse.Success);
        Assert.Contains("Failed to create shell case.", returnedResponse.ErrorMessages);
    }

    [Fact]
    public async Task CreateShellCase_ReturnsBadRequest_WhenValidationError()
    {
        var response = new CreateShellCaseCommandResponse
        {
        };
        response.SetOrUpdateErrorMessage("Validation error");

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateShellCaseCommand>(), default))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);
        var result = await controller.CreateShellCase("TestSourceSystem");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedResponse = Assert.IsType<CreateShellCaseCommandResponse>(badRequestResult.Value);
        Assert.False(returnedResponse.Success);
        Assert.Contains("Validation error", returnedResponse.ErrorMessages);
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
