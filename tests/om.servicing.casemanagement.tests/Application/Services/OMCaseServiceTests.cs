using Moq;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Services;

public class OMCaseServiceTests
{
    private readonly Mock<ILoggingService> _loggingServiceMock = new();
    private readonly Mock<IGenericRepository<OMCase>> _caseRepositoryMock = new();
    private readonly OMCaseService _service;

    public OMCaseServiceTests()
    {
        _service = new OMCaseService(_loggingServiceMock.Object, _caseRepositoryMock.Object);
    }

    [Fact]
    public async Task GetCasesForCustomerByIdentificationNumberAsync_ReturnsError_WhenIdentificationNumberIsMissing()
    {
        var result = await _service.GetCasesForCustomerByIdentificationNumberAsync("");
        Assert.Contains("Identification number is required.", result.ErrorMessages);
        Assert.False(result.Success);
        Assert.Empty(result.Data);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetCasesForCustomerByIdentificationNumberAsync_ReturnsCases_WhenIdentificationNumberIsValid()
    {
        var cases = new List<OMCase>
        {
            new OMCase { IdentificationNumber = "ID123", ReferenceNumber = "REF123", Channel = "Web" }
        };
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(cases);

        var result = await _service.GetCasesForCustomerByIdentificationNumberAsync("ID123");

        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Single(result.Data);
        Assert.Equal("ID123", result.Data[0].IdentificationNumber);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCasesForCustomerByIdentificationNumberAsync_ReturnsError_WhenRepositoryThrows()
    {
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ThrowsAsync(new ReadPersistenceException("DB error"));

        var result = await _service.GetCasesForCustomerByIdentificationNumberAsync("ID123");

        Assert.False(result.Success);
        Assert.NotEmpty(result.CustomExceptions);       
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData(null, "Open")]
    [InlineData("", "Open")]
    [InlineData("ID123", null)]
    [InlineData("ID123", "")]
    public async Task GetCasesForCustomerByIdentificationNumberAndStatusAsync_ReturnsError_WhenIdentityOrStatusIsMissing(string identityNumber, string status)
    {
        var result = await _service.GetCasesForCustomerByIdentificationNumberAndStatusAsync(identityNumber, status);
        Assert.Contains("Both identity number and status are required.", result.ErrorMessages);
        Assert.False(result.Success);
        Assert.Empty(result.Data);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetCasesForCustomerByIdentificationNumberAndStatusAsync_ReturnsCases_WhenInputIsValid()
    {
        var cases = new List<OMCase>
        {
            new OMCase { IdentificationNumber = "ID123", Status = "Open", ReferenceNumber = "REF123", Channel = "Web" }
        };
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(cases);

        var result = await _service.GetCasesForCustomerByIdentificationNumberAndStatusAsync("ID123", "Open");

        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Single(result.Data);
        Assert.Equal("ID123", result.Data[0].IdentificationNumber);
        Assert.Equal("Open", result.Data[0].Status);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCasesForCustomerByIdentificationNumberAndStatusAsync_ReturnsError_WhenRepositoryThrows()
    {
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetCasesForCustomerByIdentificationNumberAndStatusAsync("ID123", "Open");

        Assert.False(result.Success);
        Assert.NotEmpty(result.CustomExceptions);
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task GetCasesForCustomerByReferenceNumberAsync_ReturnsError_WhenReferenceNumberIsMissing()
    {
        var result = await _service.GetCasesForCustomerByReferenceNumberAsync("");
        Assert.Contains("Reference number is required.", result.ErrorMessages);
        Assert.False(result.Success);
        Assert.Empty(result.Data);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetCasesForCustomerByReferenceNumberAsync_ReturnsCases_WhenReferenceNumberIsValid()
    {
        var cases = new List<OMCase>
        {
            new OMCase { ReferenceNumber = "REF123", Channel = "Web", IdentificationNumber = "ID123" }
        };
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(cases);

        var result = await _service.GetCasesForCustomerByReferenceNumberAsync("REF123");

        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Single(result.Data);
        Assert.Equal("REF123", result.Data[0].ReferenceNumber);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCasesForCustomerByReferenceNumberAsync_ReturnsError_WhenRepositoryThrows()
    {
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetCasesForCustomerByReferenceNumberAsync("REF123");

        Assert.False(result.Success);
        Assert.NotEmpty(result.CustomExceptions);
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData(null, "Open")]
    [InlineData("", "Open")]
    [InlineData("REF123", null)]
    [InlineData("REF123", "")]
    public async Task GetCasesForCustomerByReferenceNumberAndStatusAsync_ReturnsError_WhenReferenceNumberOrStatusIsMissing(string referenceNumber, string status)
    {
        var result = await _service.GetCasesForCustomerByReferenceNumberAndStatusAsync(referenceNumber, status);
        Assert.Contains("Both reference number and status are required.", result.ErrorMessages);
        Assert.False(result.Success);
        Assert.Empty(result.Data);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetCasesForCustomerByReferenceNumberAndStatusAsync_ReturnsCases_WhenInputIsValid()
    {
        var cases = new List<OMCase>
        {
            new OMCase { ReferenceNumber = "REF123", Status = "Open", Channel = "Web", IdentificationNumber = "ID123" }
        };
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(cases);

        var result = await _service.GetCasesForCustomerByReferenceNumberAndStatusAsync("REF123", "Open");

        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        Assert.Single(result.Data);
        Assert.Equal("REF123", result.Data[0].ReferenceNumber);
        Assert.Equal("Open", result.Data[0].Status);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCasesForCustomerByReferenceNumberAndStatusAsync_ReturnsError_WhenRepositoryThrows()
    {
        _caseRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetCasesForCustomerByReferenceNumberAndStatusAsync("REF123", "Open");

        Assert.False(result.Success);
        Assert.NotEmpty(result.CustomExceptions);
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateCaseAsync_ReturnsResponseWithNoData_WhenDtoIsNull()
    {
        var result = await _service.CreateCaseAsync(null);
        Assert.NotNull(result);
        Assert.Empty(result.Data.ReferenceNumber);
        Assert.Empty(result.Data.Id);
        Assert.NotNull(result.ErrorMessages ?? new List<string>());
        Assert.False(result.Success);
        _caseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OMCase>()), Times.Never);
    }

    [Fact]
    public async Task CreateCaseAsync_SuccessfullyCreatesCase_AndSetsReferenceNumberAndId()
    {
        var dto = new OMCaseDto
        {
            Channel = "PublicWeb",
            IdentificationNumber = "ID123",
            Status = "Open"
        };

        _caseRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<OMCase>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateCaseAsync(dto);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.ReferenceNumber));
        Assert.False(string.IsNullOrWhiteSpace(result.Data.Id));
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessages ?? new List<string>());
        _caseRepositoryMock.Verify(r => r.AddAsync(It.Is<OMCase>(c =>
            c.IdentificationNumber == "ID123" &&
            c.Status == "Open" &&
            c.Channel == "PublicWeb" &&
            !string.IsNullOrWhiteSpace(c.ReferenceNumber) &&
            !string.IsNullOrWhiteSpace(c.Id)
        )), Times.Once);
    }

    [Fact]
    public async Task CreateCaseAsync_ReturnsError_WhenRepositoryThrows()
    {
        var dto = new OMCaseDto
        {
            Channel = "PublicWeb",
            IdentificationNumber = "ID123",
            Status = "Open"
        };

        _caseRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<OMCase>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _service.CreateCaseAsync(dto);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.NotEmpty(result.CustomExceptions);
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateCaseAsync_ReferenceNumberFormat_IsCorrect()
    {
        var dto = new OMCaseDto
        {
            Channel = "PublicWeb",
            IdentificationNumber = "ID123",
            Status = "Open"
        };

        _caseRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<OMCase>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateCaseAsync(dto);

        Assert.NotNull(result.Data.ReferenceNumber);
        // Should start with "CSP" for CustomerServicing + PublicWeb
        Assert.StartsWith("CSP", result.Data.ReferenceNumber);
        Assert.True(result.Data.ReferenceNumber.Length == 18);
    }

    [Fact]
    public async Task CaseExistsWithIdAsync_ReturnsFalse_WhenIdIsNullOrWhitespace()
    {
        Assert.False(await _service.CaseExistsWithIdAsync(null));
        Assert.False(await _service.CaseExistsWithIdAsync(""));
        Assert.False(await _service.CaseExistsWithIdAsync("   "));
        _caseRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CaseExistsWithIdAsync_ReturnsTrue_WhenCaseExists()
    {
        var caseId = "CASE123";
        _caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(new OMCase { Id = caseId });
        Assert.True(await _service.CaseExistsWithIdAsync(caseId));
        _caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
    }

    [Fact]
    public async Task CaseExistsWithIdAsync_ReturnsFalse_WhenCaseDoesNotExist()
    {
        var caseId = "CASE123";
        _caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync((OMCase)null);
        Assert.False(await _service.CaseExistsWithIdAsync(caseId));
        _caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
    }

    [Fact]
    public async Task CaseExistsWithIdAsync_ReturnsFalse_AndLogs_WhenRepositoryThrows()
    {
        var caseId = "CASE123";
        _caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ThrowsAsync(new Exception("DB error"));
        Assert.False(await _service.CaseExistsWithIdAsync(caseId));
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CaseExistsWithReferenceNumberAsync_ReturnsFalse_WhenReferenceNumberIsNullOrWhitespace()
    {
        Assert.False(await _service.CaseExistsWithReferenceNumberAsync(null));
        Assert.False(await _service.CaseExistsWithReferenceNumberAsync(""));
        Assert.False(await _service.CaseExistsWithReferenceNumberAsync("   "));
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task CaseExistsWithReferenceNumberAsync_ReturnsTrue_WhenCaseExists()
    {
        var referenceNumber = "REF123";
        _caseRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(new List<OMCase> { new OMCase { ReferenceNumber = referenceNumber } });
        Assert.True(await _service.CaseExistsWithReferenceNumberAsync(referenceNumber));
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task CaseExistsWithReferenceNumberAsync_ReturnsFalse_WhenCaseDoesNotExist()
    {
        var referenceNumber = "REF123";
        _caseRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(new List<OMCase>());
        Assert.False(await _service.CaseExistsWithReferenceNumberAsync(referenceNumber));
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task CaseExistsWithReferenceNumberAsync_ReturnsFalse_AndLogs_WhenRepositoryThrows()
    {
        var referenceNumber = "REF123";
        _caseRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ThrowsAsync(new Exception("DB error"));
        Assert.False(await _service.CaseExistsWithReferenceNumberAsync(referenceNumber));
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateCaseAsync_EnsuresUniqueIdAndReferenceNumber_BeforeAdd()
    {
        var dto = new OMCaseDto
        {
            Channel = "PublicWeb",
            IdentificationNumber = "ID123",
            Status = "Open"
        };

        // Simulate duplicate id and reference number on first check, then unique on second
        var duplicateId = "DUPLICATE_ID";
        var duplicateRef = "DUPLICATE_REF";
        var uniqueId = "UNIQUE_ID";
        var uniqueRef = "UNIQUE_REF";

        // Setup initial id/ref
        dto.Id = duplicateId;
        dto.ReferenceNumber = duplicateRef;

        // Setup repository to simulate duplicates, then unique
        var idCallCount = 0;
        _caseRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(() =>
            {
                idCallCount++;
                return idCallCount == 1 ? new OMCase { Id = duplicateId } : null;
            });

        var refCallCount = 0;
        _caseRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()))
            .ReturnsAsync(() =>
            {
                refCallCount++;
                return refCallCount == 1 ? new List<OMCase> { new OMCase { ReferenceNumber = duplicateRef } } : new List<OMCase>();
            });

        _caseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<OMCase>())).Returns(Task.CompletedTask);

        var result = await _service.CreateCaseAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.Success);
        _caseRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.AtLeastOnce);
        _caseRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMCase, bool>>>()), Times.AtLeastOnce);
        _caseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OMCase>()), Times.Once);
    }
}
