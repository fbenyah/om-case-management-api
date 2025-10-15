using Moq;
using om.servicing.casemanagement.application.Features.OMTransactions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMTransactions.Queries;

public class GetTransactionsForCaseByCaseIdQueryHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMTransactionService> _transactionServiceMock;
    private readonly GetTransactionsForCaseByCaseIdQueryHandler _handler;

    public GetTransactionsForCaseByCaseIdQueryHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _transactionServiceMock = new Mock<IOMTransactionService>();
        _handler = new GetTransactionsForCaseByCaseIdQueryHandler(
            _loggingServiceMock.Object,
            _transactionServiceMock.Object
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ReturnsError_WhenCaseIdIsMissing(string caseId)
    {
        var query = new GetTransactionsForCaseByCaseIdQuery { CaseId = caseId };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("The Case Id is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCaseIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsTransactions_WhenCaseIdIsValid()
    {
        var transactions = new List<OMTransactionDto>
        {
            new OMTransactionDto { ReceivedDetails = "Received", ProcessedDetails = "Processed", Status = "Completed" }
        };

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCaseIdAsync("case123", CancellationToken.None))
            .ReturnsAsync(transactions);

        var query = new GetTransactionsForCaseByCaseIdQuery { CaseId = "case123" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(transactions, result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCaseIdAsync("case123", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoTransactionsFound()
    {
        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCaseIdAsync("case456", CancellationToken.None))
            .ReturnsAsync(new List<OMTransactionDto>());

        var query = new GetTransactionsForCaseByCaseIdQuery { CaseId = "case456" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCaseIdAsync("case456", CancellationToken.None), Times.Once);
    }
}
