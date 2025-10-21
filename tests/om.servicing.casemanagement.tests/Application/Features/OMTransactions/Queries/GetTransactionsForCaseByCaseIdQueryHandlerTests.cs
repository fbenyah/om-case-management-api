using Moq;
using om.servicing.casemanagement.application.Features.OMTransactions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;
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
    public async Task Handle_ReturnsTransactions_WhenServiceReturnsSuccess()
    {
        var serviceResponse = new OMTransactionListResponse
        {
            Data = new List<OMTransactionDto>
            {
                new OMTransactionDto { ReceivedDetails = "Received", ProcessedDetails = "Processed", Status = "Completed" }
            }
        };

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCaseIdAsync("case123", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForCaseByCaseIdQuery { CaseId = "case123" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(serviceResponse.Data, result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCaseIdAsync("case123", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_PropagatesErrorsAndCustomExceptions_WhenServiceReturnsFailure()
    {
        var serviceResponse = new OMTransactionListResponse();
        serviceResponse.SetOrUpdateErrorMessages(new List<string> { "repo error" });
        serviceResponse.SetOrUpdateCustomExceptions(new List<ICustomException> { new ClientException("custom-ex") });

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCaseIdAsync("caseErr", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForCaseByCaseIdQuery { CaseId = "caseErr" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("repo error", result.ErrorMessages);
        Assert.NotNull(result.CustomExceptions);
        Assert.Contains(result.CustomExceptions, ex => ex is ClientException);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCaseIdAsync("caseErr", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyData_WhenServiceSucceedsWithNoTransactions()
    {
        var serviceResponse = new OMTransactionListResponse
        {
            Data = new List<OMTransactionDto>()
        };

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCaseIdAsync("caseEmpty", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForCaseByCaseIdQuery { CaseId = "caseEmpty" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCaseIdAsync("caseEmpty", CancellationToken.None), Times.Once);
    }
}
