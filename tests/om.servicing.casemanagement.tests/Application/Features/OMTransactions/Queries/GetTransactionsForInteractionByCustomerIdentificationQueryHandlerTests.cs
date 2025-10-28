using Moq;
using om.servicing.casemanagement.application.Features.OMTransactions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMTransactions.Queries;

public class GetTransactionsForInteractionByCustomerIdentificationQueryHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMTransactionService> _transactionServiceMock;
    private readonly GetTransactionsForInteractionByCustomerIdentificationQueryHandler _handler;

    public GetTransactionsForInteractionByCustomerIdentificationQueryHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _transactionServiceMock = new Mock<IOMTransactionService>();
        _handler = new GetTransactionsForInteractionByCustomerIdentificationQueryHandler(
            _loggingServiceMock.Object,
            _transactionServiceMock.Object
        );
    }

    [Theory]
    [InlineData(null, "int1")]
    [InlineData("", "int1")]
    [InlineData("   ", "int1")]
    public async Task Handle_ReturnsError_WhenCustomerIdentificationNumberIsMissing(string customerId, string interactionId)
    {
        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = customerId,
            InteractionId = interactionId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("Customer identification number is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<String[]?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("cust1", null)]
    [InlineData("cust1", "")]
    [InlineData("cust1", "   ")]
    public async Task Handle_ReturnsError_WhenInteractionIdIsMissing(string customerId, string interactionId)
    {
        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = customerId,
            InteractionId = interactionId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("Interaction ID is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<String[]?>(), It.IsAny<CancellationToken>()), Times.Never);
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
            .Setup(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("cust123", "int456", null, CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = "cust123",
            InteractionId = "int456"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(serviceResponse.Data, result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("cust123", "int456", null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyData_WhenServiceSucceedsWithNoTransactions()
    {
        var serviceResponse = new OMTransactionListResponse
        {
            Data = new List<OMTransactionDto>()
        };

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("custEmpty", "intEmpty", null, CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = "custEmpty",
            InteractionId = "intEmpty"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("custEmpty", "intEmpty", null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_PropagatesErrorsAndCustomExceptions_WhenServiceReturnsFailure()
    {
        OMTransactionListResponse serviceResponse = new();
        serviceResponse.SetOrUpdateErrorMessages(new List<string> { "service error" });
        serviceResponse.SetOrUpdateCustomExceptions(new List<ICustomException> { new ClientException("custom-ex") });

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("custErr", "intErr", null, CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = "custErr",
            InteractionId = "intErr"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("service error", result.ErrorMessages);
        Assert.NotNull(result.CustomExceptions);
        Assert.Contains(result.CustomExceptions, ex => ex is ClientException);
        _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("custErr", "intErr", null, CancellationToken.None), Times.Once);
    }
}
