using Moq;
using om.servicing.casemanagement.application.Features.OMTransactions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.domain.Dtos;
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
    [InlineData(null, "validInteraction")]
    [InlineData("", "validInteraction")]
    [InlineData("   ", "validInteraction")]
    [InlineData("validCustomer", null)]
    [InlineData("validCustomer", "")]
    [InlineData("validCustomer", "   ")]
    public async Task Handle_ReturnsError_WhenCustomerIdentificationOrInteractionIdIsMissing(string customerId, string interactionId)
    {
        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = customerId,
            InteractionId = interactionId
        };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        if (string.IsNullOrWhiteSpace(customerId))
        {
            Assert.Contains("Customer identification number is required.", result.ErrorMessages);
            _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        else
        {
            Assert.Contains("Interaction ID is required.", result.ErrorMessages);
            _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_ReturnsTransactions_WhenInputIsValid()
    {
        var transactions = new List<OMTransactionDto>
        {
            new OMTransactionDto { ReceivedDetails = "Received", ProcessedDetails = "Processed", Status = "Completed" }
        };

        _transactionServiceMock
            .Setup(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("cust123", "int456"))
            .ReturnsAsync(transactions);

        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = "cust123",
            InteractionId = "int456"
        };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(transactions, result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("cust123", "int456"), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoTransactionsFound()
    {
        _transactionServiceMock
            .Setup(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("cust789", "int101"))
            .ReturnsAsync(new List<OMTransactionDto>());

        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = "cust789",
            InteractionId = "int101"
        };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Empty(result.Data);
        _transactionServiceMock.Verify(s => s.GetTransactionsForInteractionByCustomerIdentificationAsync("cust789", "int101"), Times.Once);
    }
}
