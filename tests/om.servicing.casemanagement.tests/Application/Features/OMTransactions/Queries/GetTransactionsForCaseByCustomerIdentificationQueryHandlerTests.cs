using Moq;
using om.servicing.casemanagement.application.Features.OMTransactions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMTransactions.Queries;

public class GetTransactionsForCaseByCustomerIdentificationQueryHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMTransactionService> _transactionServiceMock;
    private readonly GetTransactionsForCaseByCustomerIdentificationQueryHandler _handler;

    public GetTransactionsForCaseByCustomerIdentificationQueryHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _transactionServiceMock = new Mock<IOMTransactionService>();
        _handler = new GetTransactionsForCaseByCustomerIdentificationQueryHandler(
            _loggingServiceMock.Object,
            _transactionServiceMock.Object
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ReturnsError_WhenCustomerIdentificationNumberIsMissing(string customerId)
    {
        var query = new GetTransactionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = customerId };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("Customer identification number is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCustomerIdentificationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsTransactions_WhenServiceReturnsSuccess()
    {
        var serviceResponse = new OMTransactionListResponse
        {
            Data = new List<OMTransactionDto>
            {
                new OMTransactionDto { ReceivedDetails = "R", ProcessedDetails = "P", Status = "Completed" }
            }
        };

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCustomerIdentificationAsync("cust123", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "cust123" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(serviceResponse.Data, result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCustomerIdentificationAsync("cust123", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyData_WhenServiceSucceedsWithNoTransactions()
    {
        var serviceResponse = new OMTransactionListResponse
        {
            Data = new List<OMTransactionDto>()
        };

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCustomerIdentificationAsync("custEmpty", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "custEmpty" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCustomerIdentificationAsync("custEmpty", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_PropagatesErrorsAndCustomExceptions_WhenServiceReturnsFailure()
    {
        OMTransactionListResponse serviceResponse = new();
        serviceResponse.SetOrUpdateErrorMessages(new List<string> { "service error" });
        serviceResponse.SetOrUpdateCustomExceptions(new List<ICustomException> { new ClientException("custom-ex") });

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForCaseByCustomerIdentificationAsync("custErr", CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "custErr" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("service error", result.ErrorMessages);
        Assert.NotNull(result.CustomExceptions);
        Assert.Contains(result.CustomExceptions, ex => ex is ClientException);
        _transactionServiceMock.Verify(s => s.GetTransactionsForCaseByCustomerIdentificationAsync("custErr", CancellationToken.None), Times.Once);
    }
}
