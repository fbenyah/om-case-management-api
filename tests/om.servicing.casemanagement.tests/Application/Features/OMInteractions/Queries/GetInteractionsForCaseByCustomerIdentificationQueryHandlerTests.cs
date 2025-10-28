using Moq;
using om.servicing.casemanagement.application.Features.OMInteractions.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;
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
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCustomerIdentificationAsync(It.IsAny<string>(), It.IsAny<String[]?>(), It.IsAny<CancellationToken>()), Times.Never);
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
            .Setup(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("cust123", null, CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetInteractionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "cust123" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(serviceResponse.Data, result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("cust123", null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_PropagatesErrorsAndCustomExceptions_WhenServiceReturnsFailure()
    {
        var serviceResponse = new OMInteractionListResponse();
        serviceResponse.SetOrUpdateErrorMessages(new List<string> { "case svc error" });
        serviceResponse.SetOrUpdateCustomExceptions(new List<ICustomException> { new ClientException("custom-ex") });

        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("custErr", null, CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetInteractionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "custErr" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("case svc error", result.ErrorMessages);
        Assert.NotNull(result.CustomExceptions);
        Assert.Contains(result.CustomExceptions, ex => ex is ClientException);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("custErr", null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyData_WhenServiceSucceedsWithNoInteractions()
    {
        var serviceResponse = new OMInteractionListResponse
        {
            Data = new List<OMInteractionDto>()
        };

        _interactionServiceMock
            .Setup(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("custEmpty", null, CancellationToken.None))
            .ReturnsAsync(serviceResponse);

        var query = new GetInteractionsForCaseByCustomerIdentificationQuery { CustomerIdentificationNumber = "custEmpty" };
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data);
        _interactionServiceMock.Verify(s => s.GetInteractionsForCaseByCustomerIdentificationAsync("custEmpty", null, CancellationToken.None), Times.Once);
    }
}
