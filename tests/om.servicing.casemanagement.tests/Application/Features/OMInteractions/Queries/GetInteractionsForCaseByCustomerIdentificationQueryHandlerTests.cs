using Moq;
using om.servicing.casemanagement.application.Features.OMInteractions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMInteractions.Queries;

public class GetInteractionsForCaseByCustomerIdentificationQueryHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMInteractionService> _interactionServiceMock;
    private readonly GetInteractionsForCaseByCustomerIdentificationQueryHandler _handler;

    public GetInteractionsForCaseByCustomerIdentificationQueryHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _interactionServiceMock = new Mock<IOMInteractionService>();
        _handler = new GetInteractionsForCaseByCustomerIdentificationQueryHandler(
            _loggingServiceMock.Object,
            _interactionServiceMock.Object
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ReturnsError_WhenCustomerIdentificationNumberIsMissing(string customerId)
    {
        var query = new GetInteractionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = customerId };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("Customer Identification Number is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCustomerIdentificationAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsInteractions_WhenCustomerIdentificationNumberIsValid()
    {
        var interactions = new List<OMInteractionDto>
    {
        new OMInteractionDto { Notes = "Interaction1", Status = "Active" }
    };

        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("cust123", CancellationToken.None))
            .ReturnsAsync(interactions);

        var query = new GetInteractionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "cust123" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(interactions, result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("cust123", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoInteractionsFound()
    {
        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("cust456", CancellationToken.None))
            .ReturnsAsync(new List<OMInteractionDto>());

        var query = new GetInteractionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "cust456" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Empty(result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("cust456", CancellationToken.None), Times.Once);
    }
}
