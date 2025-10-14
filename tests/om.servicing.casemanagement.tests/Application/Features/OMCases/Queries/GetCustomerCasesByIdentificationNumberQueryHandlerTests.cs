using Moq;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenIdentificationNumberIsNullOrWhitespace()
    {
        var caseServiceMock = new Mock<IOMCaseService>();
        var loggingServiceMock = new Mock<OM.RequestFramework.Core.Logging.ILoggingService>();
        var handler = new GetCustomerCasesByIdentificationNumberQueryHandler(loggingServiceMock.Object, caseServiceMock.Object);

        var query = new GetCustomerCasesByIdentificationNumberQuery { IdentificationNumber = "   " };
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Identification number is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        caseServiceMock.Verify(s => s.GetCasesForCustomer(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsCases_WhenIdentificationNumberIsValid()
    {
        var caseServiceMock = new Mock<IOMCaseService>();
        var loggingServiceMock = new Mock<OM.RequestFramework.Core.Logging.ILoggingService>();
        var cases = new List<OMCaseDto>
        {
            new OMCaseDto { Channel = "Email", ReferenceNumber = "ref1234", IdentificationNumber = "123", Status = "Open" },
            new OMCaseDto { Channel = "Phone", ReferenceNumber = "ref6789", IdentificationNumber = "123", Status = "Closed" }
        };
        caseServiceMock.Setup(s => s.GetCasesForCustomer("123")).ReturnsAsync(cases);

        var handler = new GetCustomerCasesByIdentificationNumberQueryHandler(loggingServiceMock.Object, caseServiceMock.Object);
        var query = new GetCustomerCasesByIdentificationNumberQuery { IdentificationNumber = "123" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Email", result.Data[0].Channel);
        Assert.Equal("Phone", result.Data[1].Channel);
        Assert.Equal("ref1234", result.Data[0].ReferenceNumber);
        Assert.Equal("ref6789", result.Data[1].ReferenceNumber);
        caseServiceMock.Verify(s => s.GetCasesForCustomer("123"), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoCasesFound()
    {
        var caseServiceMock = new Mock<IOMCaseService>();
        var loggingServiceMock = new Mock<OM.RequestFramework.Core.Logging.ILoggingService>();
        caseServiceMock.Setup(s => s.GetCasesForCustomer("456")).ReturnsAsync(new List<OMCaseDto>());

        var handler = new GetCustomerCasesByIdentificationNumberQueryHandler(loggingServiceMock.Object, caseServiceMock.Object);
        var query = new GetCustomerCasesByIdentificationNumberQuery { IdentificationNumber = "456" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Empty(result.Data);
        caseServiceMock.Verify(s => s.GetCasesForCustomer("456"), Times.Once);
    }
}
