using Moq;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByReferenceNumberQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenReferenceNumberIsNullOrWhitespace()
    {
        var caseServiceMock = new Mock<IOMCaseService>();
        var loggingServiceMock = new Mock<OM.RequestFramework.Core.Logging.ILoggingService>();
        var handler = new GetCustomerCasesByReferenceNumberQueryHandler(loggingServiceMock.Object, caseServiceMock.Object);

        var query = new GetCustomerCasesByReferenceNumberQuery { ReferenceNumber = "   " };
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Reference number is required.", result.ErrorMessages);
        Assert.Empty(result.Data);
        caseServiceMock.Verify(s => s.GetCasesForCustomerByReferenceNumberAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsCases_WhenReferenceNumberIsValid()
    {
        var caseServiceMock = new Mock<IOMCaseService>();
        var loggingServiceMock = new Mock<OM.RequestFramework.Core.Logging.ILoggingService>();

        var cases = new OMCaseListResponse()
        {
            Data = new List<OMCaseDto>
            {
                new OMCaseDto { Channel = "Email", ReferenceNumber = "ref1234", IdentificationNumber = "123", Status = "Open" },
                new OMCaseDto { Channel = "Phone", ReferenceNumber = "ref1234", IdentificationNumber = "123", Status = "Closed" }
            }
        };

        caseServiceMock.Setup(s => s.GetCasesForCustomerByReferenceNumberAsync("ref1234", CancellationToken.None)).ReturnsAsync(cases);

        var handler = new GetCustomerCasesByReferenceNumberQueryHandler(loggingServiceMock.Object, caseServiceMock.Object);
        var query = new GetCustomerCasesByReferenceNumberQuery { ReferenceNumber = "ref1234" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Email", result.Data[0].Channel);
        Assert.Equal("Phone", result.Data[1].Channel);
        Assert.Equal("ref1234", result.Data[0].ReferenceNumber);
        Assert.Equal("ref1234", result.Data[1].ReferenceNumber);
        caseServiceMock.Verify(s => s.GetCasesForCustomerByReferenceNumberAsync("ref1234", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoCasesFound()
    {
        var caseServiceMock = new Mock<IOMCaseService>();
        var loggingServiceMock = new Mock<OM.RequestFramework.Core.Logging.ILoggingService>();
        caseServiceMock.Setup(s => s.GetCasesForCustomerByReferenceNumberAsync("456", CancellationToken.None)).ReturnsAsync(new OMCaseListResponse());

        var handler = new GetCustomerCasesByReferenceNumberQueryHandler(loggingServiceMock.Object, caseServiceMock.Object);
        var query = new GetCustomerCasesByReferenceNumberQuery { ReferenceNumber = "456" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Empty(result.Data);
        caseServiceMock.Verify(s => s.GetCasesForCustomerByReferenceNumberAsync("456", CancellationToken.None), Times.Once);
    }
}
