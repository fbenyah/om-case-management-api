using Moq;
using om.servicing.casemanagement.application.Features.OMInteractions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.domain.Dtos;
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
    public async Task Handle_ReturnsInteractions_WhenCaseIdIsValid()
    {
        var interactions = new List<OMInteractionDto>
        {
            new OMInteractionDto { Notes = "Interaction1", Status = "Active" }
        };

        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCaseIdAsync("case123", CancellationToken.None))
            .ReturnsAsync(interactions);

        var query = new GetInteractionsForCaseByCaseIdQuery { CaseId = "case123" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(interactions, result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCaseIdAsync("case123", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoInteractionsFound()
    {
        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCaseIdAsync("case456", CancellationToken.None))
            .ReturnsAsync(new List<OMInteractionDto>());

        var query = new GetInteractionsForCaseByCaseIdQuery { CaseId = "case456" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Empty(result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCaseIdAsync("case456", CancellationToken.None), Times.Once);
    }
}
