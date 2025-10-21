using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using om.servicing.casemanagement.api.Controllers.V1;
using om.servicing.casemanagement.application.Features.OMCases.Commands;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.tests.Api.Controllers.V1;

public class CaseControllerTests
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
        var result = await controller.GetCustomerCasesByIdentification(CaseChannel.Branch, "123");

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
        var result = await controller.GetCustomerCasesByIdentification(CaseChannel.SecureWeb, "123");

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
        await controller.GetCustomerCasesByIdentification(CaseChannel.Connect, "ABC123");

        // Assert
        mediatorMock.Verify(m => m.Send(
            It.Is<GetCustomerCasesByIdentificationNumberQuery>(q => q.IdentificationNumber == "ABC123"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCustomerCasesByReference_ReturnsOk_WhenResponseIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByReferenceNumberResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByReference(CaseChannel.Branch, "123");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByReference_ReturnsBadRequest_WhenResponseIsNotSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByReferenceNumberResponse { };
        response.SetOrUpdateErrorMessage("Error occurred");

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByReference(CaseChannel.SecureWeb, "123");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(response, badRequestResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByReference_PassesCorrectQueryToMediator()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByReferenceNumberResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        await controller.GetCustomerCasesByReference(CaseChannel.Connect, "ABC123");

        // Assert
        mediatorMock.Verify(m => m.Send(
            It.Is<GetCustomerCasesByReferenceNumberQuery>(q => q.ReferenceNumber == "ABC123"),
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
        var result = await controller.GetCustomerCasesByIdentificationAndStatus(CaseChannel.IMIConnect, "123", "Open");

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
        var result = await controller.GetCustomerCasesByIdentificationAndStatus(CaseChannel.AdviserWorkBench, "123", "Open");

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
        await controller.GetCustomerCasesByIdentificationAndStatus(CaseChannel.AgentWorkBench, "ABC123", "Closed");

        // Assert
        mediatorMock.Verify(m => m.Send(
            It.Is<GetCustomerCasesByIdentificationNumberAndStatusQuery>(q => q.IdentificationNumber == "ABC123" && q.Status == "Closed"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCustomerCasesByReferenceAndStatus_ReturnsOk_WhenResponseIsSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByReferenceNumberAndStatusResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByReferenceNumberAndStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByReferenceAndStatus(CaseChannel.IMIConnect, "123", "Open");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByReferenceAndStatus_ReturnsBadRequest_WhenResponseIsNotSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByReferenceNumberAndStatusResponse { };
        response.SetOrUpdateErrorMessage("Error occurred");

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByReferenceNumberAndStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        var result = await controller.GetCustomerCasesByReferenceAndStatus(CaseChannel.AdviserWorkBench, "123", "Open");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(response, badRequestResult.Value);
    }

    [Fact]
    public async Task GetCustomerCasesByReferenceAndStatus_PassesCorrectQueryToMediator()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var response = new GetCustomerCasesByReferenceNumberAndStatusResponse { };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCustomerCasesByReferenceNumberAndStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);

        // Act
        await controller.GetCustomerCasesByReferenceAndStatus(CaseChannel.AgentWorkBench, "ABC123", "Closed");

        // Assert
        mediatorMock.Verify(m => m.Send(
            It.Is<GetCustomerCasesByReferenceNumberAndStatusQuery>(q => q.ReferenceNumber == "ABC123" && q.Status == "Closed"),
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
        var result = await controller.CreateShellCase(CaseChannel.AgentWorkBench);

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
        var result = await controller.CreateShellCase(CaseChannel.AdviserWorkBench);

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
        var result = await controller.CreateShellCase(CaseChannel.Unknown);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedResponse = Assert.IsType<CreateShellCaseCommandResponse>(badRequestResult.Value);
        Assert.False(returnedResponse.Success);
        Assert.Contains("Validation error", returnedResponse.ErrorMessages);
    }

    [Fact]
    public async Task CreateOMCase_ReturnsOk_WhenSuccess()
    {
        var response = new CreateOMCaseCommandResponse
        {
            Data = new BasicCaseCreateResponse { Id = "CASE123", ReferenceNumber = "REF456" }
        };

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateOMCaseCommand>(), default))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);
        var result = await controller.CreateOMCase(CaseChannel.PublicWeb, "ID123");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<CreateOMCaseCommandResponse>(okResult.Value);
        Assert.True(returnedResponse.Success);
        Assert.Equal("CASE123", returnedResponse.Data.Id);
        Assert.Equal("REF456", returnedResponse.Data.ReferenceNumber);
    }

    [Fact]
    public async Task CreateOMCase_ReturnsBadRequest_WhenNotSuccess()
    {
        var response = new CreateOMCaseCommandResponse
        {
        };
        response.SetOrUpdateErrorMessage("Failed to create case.");

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateOMCaseCommand>(), default))
            .ReturnsAsync(response);

        var controller = CreateControllerWithMediator(mediatorMock);
        var result = await controller.CreateOMCase(CaseChannel.PublicWeb, "ID123");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedResponse = Assert.IsType<CreateOMCaseCommandResponse>(badRequestResult.Value);
        Assert.False(returnedResponse.Success);
        Assert.Contains("Failed to create case.", returnedResponse.ErrorMessages);
    }

    private CaseController CreateControllerWithMediator(Mock<IMediator> mediatorMock)
    {
        var controller = new CaseController();

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
