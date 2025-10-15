using Moq;
using om.servicing.casemanagement.application.Features.OMCases.Commands;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Commands;

public class CreateShellCaseCommandHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMCaseService> _caseServiceMock;
    private readonly CreateShellCaseCommandHandler _handler;

    public CreateShellCaseCommandHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _caseServiceMock = new Mock<IOMCaseService>();
        _handler = new CreateShellCaseCommandHandler(
            _loggingServiceMock.Object,
            _caseServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsSuccessResponse_WhenCaseIsCreated()
    {
        var basicResponse = new BasicCaseCreateResponse
        {
            Id = "CASE123",
            ReferenceNumber = "REF456"
        };
        var serviceResponse = new OMCaseCreateResponse
        {
            Data = basicResponse
        };

        _caseServiceMock
            .Setup(s => s.CreateCaseAsync(It.IsAny<OMCaseDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var command = new CreateShellCaseCommand();
        var result = await _handler.Handle(command, default);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("CASE123", result.Data.Id);
        Assert.Equal("REF456", result.Data.ReferenceNumber);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenCaseServiceFails()
    {
        var serviceResponse = new OMCaseCreateResponse { };
        serviceResponse.SetOrUpdateErrorMessage("Service error");

        _caseServiceMock
            .Setup(s => s.CreateCaseAsync(It.IsAny<OMCaseDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var command = new CreateShellCaseCommand();
        var result = await _handler.Handle(command, default);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Failed to create shell case.", result.ErrorMessages);
        Assert.Contains("Service error", result.ErrorMessages);
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WithCustomExceptions_WhenCaseServiceFails()
    {
        var serviceResponse = new OMCaseCreateResponse { };
        serviceResponse.SetOrUpdateErrorMessage("Service error");
        serviceResponse.SetOrUpdateCustomException(new ClientException("Custom error"));

        _caseServiceMock
            .Setup(s => s.CreateCaseAsync(It.IsAny<OMCaseDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var command = new CreateShellCaseCommand();
        var result = await _handler.Handle(command, default);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Failed to create shell case.", result.ErrorMessages);
        Assert.Contains("Service error", result.ErrorMessages);
        Assert.NotEmpty(result.CustomExceptions);
    }
}
