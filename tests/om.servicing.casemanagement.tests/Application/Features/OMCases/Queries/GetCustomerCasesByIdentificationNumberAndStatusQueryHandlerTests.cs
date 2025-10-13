using Moq;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberAndStatusQueryHandlerTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IOMCaseService> _caseServiceMock;
    private readonly GetCustomerCasesByIdentificationNumberAndStatusQueryHandler _handler;

    public GetCustomerCasesByIdentificationNumberAndStatusQueryHandlerTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _caseServiceMock = new Mock<IOMCaseService>();
        _handler = new GetCustomerCasesByIdentificationNumberAndStatusQueryHandler(
            _loggingServiceMock.Object,
            _caseServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenIdentificationNumberIsMissing()
    {
        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "",
            Status = "Open"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("Identification number is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenStatusIsMissing()
    {
        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "123456",
            Status = ""
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("Status of case(s) is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_ReturnsCases_WhenInputIsValid()
    {
        var cases = new List<OMCaseDto>
        {
            new OMCaseDto { IdentificationNumber = "123456", Status = "Open", Channel = "Web" }
        };

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByStatusAsync("123456", "Open"))
            .ReturnsAsync(cases);

        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "123456",
            Status = "Open"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages);
        Assert.Equal(cases, result.Data);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenServiceReturnsNoCases()
    {
        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByStatusAsync("123456", "Closed"))
            .ReturnsAsync(new List<OMCaseDto>());

        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "123456",
            Status = "Closed"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages);
        Assert.Empty(result.Data);
    }
}
