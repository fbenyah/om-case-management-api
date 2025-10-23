using Moq;
using om.servicing.casemanagement.application.Features.OMInteractions.Commands;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Exceptions.Client;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMInteractions.Commands;

public class CreateOMInteractionCommandHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMCaseService> _caseServiceMock;
    private readonly Mock<IOMInteractionService> _interactionServiceMock;
    private readonly CreateOMInteractionCommandHandler _handler;

    public CreateOMInteractionCommandHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _caseServiceMock = new Mock<IOMCaseService>();
        _interactionServiceMock = new Mock<IOMInteractionService>();
        _interactionServiceMock = new Mock<IOMInteractionService>();
        _handler = new CreateOMInteractionCommandHandler(
            _loggingServiceMock.Object,
            _caseServiceMock.Object,
            _interactionServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenCaseServiceFails()
    {
        var caseServiceResponse = new OMCaseListResponse();
        caseServiceResponse.SetOrUpdateErrorMessage("Case service error");

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByCaseId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseServiceResponse);

        var command = new CreateOMInteractionCommand { CaseId = "CASE1" };
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Case service error", result.ErrorMessages);
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenNoCaseFound()
    {
        var caseServiceResponse = new OMCaseListResponse
        {
            Data = new List<OMCaseDto>()
        };

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByCaseId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseServiceResponse);

        var command = new CreateOMInteractionCommand { CaseId = "CASE_NOT_FOUND" };
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains($"No case found for CaseId: {command.CaseId}", result.ErrorMessages);
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenMultipleCasesFound()
    {
        var caseServiceResponse = new OMCaseListResponse
        {
            Data = new List<OMCaseDto> { new OMCaseDto(), new OMCaseDto() }
        };

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByCaseId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseServiceResponse);

        var command = new CreateOMInteractionCommand { CaseId = "DUP_CASE" };
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains($"Multiple cases found for CaseId: {command.CaseId}", result.ErrorMessages);
        Assert.NotNull(result.CustomExceptions);
        Assert.Single(result.CustomExceptions);
        Assert.IsType<ConflictException>(result.CustomExceptions.First());
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenInteractionServiceFails()
    {
        var caseDto = new OMCaseDto();
        var caseServiceResponse = new OMCaseListResponse
        {
            Data = new List<OMCaseDto> { caseDto }
        };

        var interactionServiceResponse = new OMInteractionCreateResponse();
        interactionServiceResponse.SetOrUpdateErrorMessage("Interaction service error");

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByCaseId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseServiceResponse);

        _interactionServiceMock
            .Setup(s => s.CreateInteractionAsync(It.IsAny<OMInteractionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interactionServiceResponse);

        var command = new CreateOMInteractionCommand { CaseId = "CASE1", Notes = "notes" };
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Interaction service error", result.ErrorMessages);
    }

    [Fact]
    public async Task Handle_ReturnsSuccessResponse_WhenInteractionCreated()
    {
        var caseDto = new OMCaseDto { IdentificationNumber = "ID1" };
        var caseServiceResponse = new OMCaseListResponse
        {
            Data = new List<OMCaseDto> { caseDto }
        };

        var basicCreate = new BasicInteractionCreateResponse
        {
            Id = "INT123",
            ReferenceNumber = "REFINT",
            CaseId = "CASE1",
            CaseReferenceNumber = "CREF"
        };

        var interactionServiceResponse = new OMInteractionCreateResponse
        {
            Data = basicCreate
        };

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByCaseId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseServiceResponse);

        _interactionServiceMock
            .Setup(s => s.CreateInteractionAsync(It.IsAny<OMInteractionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interactionServiceResponse);

        var command = new CreateOMInteractionCommand { CaseId = "CASE1", Notes = "some notes" };
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("INT123", result.Data.Id);
        Assert.Equal("REFINT", result.Data.ReferenceNumber);
    }
}
