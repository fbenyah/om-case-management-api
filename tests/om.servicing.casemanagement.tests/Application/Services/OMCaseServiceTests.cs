using Moq;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Entities;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Services;

public class OMCaseServiceTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly Mock<IGenericRepository<OMCase>> _repoMock;
    private readonly OMCaseService _service;

    public OMCaseServiceTests()
    {
        _loggingServiceMock = new Mock<ILoggingService>();
        _repoMock = new Mock<IGenericRepository<OMCase>>();
        _service = new OMCaseService(_loggingServiceMock.Object, _repoMock.Object);
    }

    [Fact]
    public async Task GetCasesForCustomer_NullOrWhitespaceIdentity_ReturnsEmptyList()
    {
        var result1 = await _service.GetCasesForCustomer(null);
        var result2 = await _service.GetCasesForCustomer("");
        var result3 = await _service.GetCasesForCustomer("   ");

        Assert.Empty(result1);
        Assert.Empty(result2);
        Assert.Empty(result3);
        _repoMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetCasesForCustomer_ValidIdentity_ReturnsMappedCases()
    {
        var cases = new List<OMCase>
        {
            new OMCase { IdentificationNumber = "123", Channel = "Email", Status = "Open" },
            new OMCase { IdentificationNumber = "123", Channel = "Phone", Status = "Closed" }
        };
        _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(cases);

        var result = await _service.GetCasesForCustomer("123");

        Assert.Equal(2, result.Count);
        Assert.All(result, dto => Assert.Equal("123", dto.IdentificationNumber));
    }

    [Fact]
    public async Task GetCasesForCustomerByStatusAsync_NullOrWhitespaceIdentityOrStatus_ReturnsEmptyList()
    {
        var result1 = await _service.GetCasesForCustomerByStatusAsync(null, "Open");
        var result2 = await _service.GetCasesForCustomerByStatusAsync("", "Open");
        var result3 = await _service.GetCasesForCustomerByStatusAsync("123", null);
        var result4 = await _service.GetCasesForCustomerByStatusAsync("123", "");
        var result5 = await _service.GetCasesForCustomerByStatusAsync("   ", "Open");
        var result6 = await _service.GetCasesForCustomerByStatusAsync("123", "   ");

        Assert.Empty(result1);
        Assert.Empty(result2);
        Assert.Empty(result3);
        Assert.Empty(result4);
        Assert.Empty(result5);
        Assert.Empty(result6);
        _repoMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetCasesForCustomerByStatusAsync_ValidIdentityAndStatus_ReturnsMappedCases()
    {
        var cases = new List<OMCase>
        {
            new OMCase { IdentificationNumber = "123", Channel = "Email", Status = "Open" }
        };
        _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(cases);

        var result = await _service.GetCasesForCustomerByStatusAsync("123", "Open");

        Assert.Single(result);
        Assert.Equal("Email", result[0].Channel);
    }
}
