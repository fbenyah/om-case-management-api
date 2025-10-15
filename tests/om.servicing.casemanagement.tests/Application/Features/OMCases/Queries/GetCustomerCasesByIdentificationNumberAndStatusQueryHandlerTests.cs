using Moq;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ReturnsError_WhenIdentificationNumberIsMissing(string identificationNumber)
    {
        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = identificationNumber,
            Status = "Open"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("Identification number is required.", result.ErrorMessages);
        Assert.False(result.Success);
        Assert.Empty(result.Data);
        _caseServiceMock.Verify(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ReturnsError_WhenStatusIsMissing(string status)
    {
        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "123456",
            Status = status
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("Status of case(s) is required.", result.ErrorMessages);
        Assert.False(result.Success);
        Assert.Empty(result.Data);
        _caseServiceMock.Verify(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsCases_WhenInputIsValid()
    {
        var caseListResponse = new OMCaseListResponse
        {
            Data = new List<OMCaseDto>
            {
                new OMCaseDto { IdentificationNumber = "123456", ReferenceNumber = "ref1234", Status = "Open", Channel = "Web" }
            }
        };

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync("123456", "Open", CancellationToken.None))
            .ReturnsAsync(caseListResponse);

        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "123456",
            Status = "Open"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(caseListResponse.Data, result.Data);
        _caseServiceMock.Verify(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync("123456", "Open", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenServiceResponseIsNotSuccess()
    {
        var caseListResponse = new OMCaseListResponse
        {
            Data = new List<OMCaseDto>()
        };
        caseListResponse.SetOrUpdateErrorMessage("Service error");

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync("123456", "Open", CancellationToken.None))
            .ReturnsAsync(caseListResponse);

        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "123456",
            Status = "Open"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Service error", result.ErrorMessages);
        Assert.Empty(result.Data);
        _caseServiceMock.Verify(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync("123456", "Open", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenServiceReturnsNoCases()
    {
        var caseListResponse = new OMCaseListResponse
        {
            Data = new List<OMCaseDto>()
        };

        _caseServiceMock
            .Setup(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync("123456", "Closed", CancellationToken.None))
            .ReturnsAsync(caseListResponse);

        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = "123456",
            Status = "Closed"
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Empty(result.Data);
        _caseServiceMock.Verify(s => s.GetCasesForCustomerByIdentificationNumberAndStatusAsync("123456", "Closed", CancellationToken.None), Times.Once);
    }
}
