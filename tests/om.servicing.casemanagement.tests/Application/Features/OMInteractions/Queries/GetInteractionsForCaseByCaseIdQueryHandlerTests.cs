using Moq;
using om.servicing.casemanagement.application.Features.OMInteractions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMInteractions.Queries;

public class GetInteractionsForCaseByCaseIdQueryHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMInteractionService> _interactionServiceMock;
    private readonly GetInteractionsForCaseByCaseIdQueryHandler _handler;

    public GetInteractionsForCaseByCaseIdQueryHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _interactionServiceMock = new Mock<IOMInteractionService>();
        _handler = new GetInteractionsForCaseByCaseIdQueryHandler(
            _loggingServiceMock.Object,
            _interactionServiceMock.Object
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ReturnsError_WhenCaseIdIsMissing(string caseId)
    {
        var query = new GetInteractionsForCaseByCaseIdQuery { CaseId = caseId };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("Case Id is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCaseIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsInteractions_WhenServiceReturnsSuccess()
    {
        var serviceResponse = new OMInteractionListResponse
        {
            Data = new List<OMInteractionDto>
            {
                new OMInteractionDto { Notes = "Interaction1", Status = "Active" }
            }
        };

        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCaseIdAsync("case123", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetInteractionsForCaseByCaseIdQuery { CaseId = "case123" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(serviceResponse.Data, result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCaseIdAsync("case123", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_PropagatesErrorsAndCustomExceptions_WhenServiceReturnsFailure()
    {
        var serviceResponse = new OMInteractionListResponse();
        serviceResponse.SetOrUpdateErrorMessages(new List<string> { "repo error", "other error" });
        serviceResponse.SetOrUpdateCustomExceptions(new List<ICustomException> { new ClientException("custom-ex") });

        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCaseIdAsync("caseErr", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetInteractionsForCaseByCaseIdQuery { CaseId = "caseErr" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("repo error", result.ErrorMessages);
        Assert.Contains("other error", result.ErrorMessages);
        Assert.NotNull(result.CustomExceptions);
        Assert.Contains(result.CustomExceptions, ex => ex is ClientException);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCaseIdAsync("caseErr", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyData_WhenServiceSucceedsWithNoInteractions()
    {
        var serviceResponse = new OMInteractionListResponse
        {
            Data = new List<OMInteractionDto>()
        };

        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCaseIdAsync("caseEmpty", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetInteractionsForCaseByCaseIdQuery { CaseId = "caseEmpty" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCaseIdAsync("caseEmpty", CancellationToken.None), Times.Once);
    }
}
