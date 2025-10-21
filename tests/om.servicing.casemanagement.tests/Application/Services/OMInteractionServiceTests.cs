using Moq;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Services;

public class OMInteractionServiceTests
{
    private readonly Mock<IOMCaseService> _mockCaseService;
    private readonly Mock<ILoggingService> _loggingServiceMock = new();
    private readonly Mock<IGenericRepository<OMInteraction>> _mockInteractionRepo;
    private readonly OMInteractionService _service;

    public OMInteractionServiceTests()
    {
        _mockCaseService = new Mock<IOMCaseService>();
        _mockInteractionRepo = new Mock<IGenericRepository<OMInteraction>>();
        _service = new OMInteractionService(_mockCaseService.Object, _loggingServiceMock.Object, _mockInteractionRepo.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetInteractionsForCaseByCaseIdAsync_ReturnsEmptyList_WhenCaseIdIsNullOrWhitespace(string caseId)
    {
        var result = await _service.GetInteractionsForCaseByCaseIdAsync(caseId);
        Assert.Empty(result);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCaseIdAsync_ReturnsEmptyList_WhenNoInteractionsFound()
    {
        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<OMInteraction>());

        var result = await _service.GetInteractionsForCaseByCaseIdAsync("case123");
        Assert.Empty(result);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCaseIdAsync_ReturnsDtoList_WhenInteractionsExist()
    {
        var interactions = new List<OMInteraction>
    {
        new OMInteraction { CaseId = "case123", Notes = "Note1", Status = "Active" }
    };

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interactions);

        var result = await _service.GetInteractionsForCaseByCaseIdAsync("case123");

        Assert.Single(result);
        Assert.Equal("Note1", result[0].Notes);
        Assert.Equal("Active", result[0].Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetInteractionsForCaseByCustomerIdentificationAsync_ReturnsEmptyList_WhenCustomerIdIsNullOrWhitespace(string customerId)
    {
        var result = await _service.GetInteractionsForCaseByCustomerIdentificationAsync(customerId);
        Assert.Empty(result);
        _mockCaseService.Verify(s => s.GetCasesForCustomerByIdentificationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCustomerIdentificationAsync_ReturnsEmptyList_WhenNoCasesFound()
    {
        _mockCaseService
            .Setup(s => s.GetCasesForCustomerByIdentificationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OMCaseListResponse());

        var result = await _service.GetInteractionsForCaseByCustomerIdentificationAsync("cust123");
        Assert.Empty(result);
        _mockCaseService.Verify(s => s.GetCasesForCustomerByIdentificationNumberAsync("cust123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCustomerIdentificationAsync_ReturnsAggregatedInteractions()
    {
        var cases = new OMCaseListResponse
        {
            Data = new List<OMCaseDto>
            {
                new OMCaseDto { Channel = "Web", IdentificationNumber = "cust123", Status = "Open", Id = "case1" },
                new OMCaseDto { Channel = "Phone", IdentificationNumber = "cust123", Status = "Closed", Id = "case2" }
            }
        };

        _mockCaseService
            .Setup(s => s.GetCasesForCustomerByIdentificationNumberAsync("cust123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cases);

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(expr => expr.Compile().Invoke(new OMInteraction { CaseId = "case1" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { CaseId = "case1", Notes = "Note1", Status = "Active" } });

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(expr => expr.Compile().Invoke(new OMInteraction { CaseId = "case2" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { CaseId = "case2", Notes = "Note2", Status = "Closed" } });

        var result = await _service.GetInteractionsForCaseByCustomerIdentificationAsync("cust123", It.IsAny<CancellationToken>());
        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Notes == "Note1");
        Assert.Contains(result, i => i.Notes == "Note2");
        _mockCaseService.Verify(s => s.GetCasesForCustomerByIdentificationNumberAsync("cust123", It.IsAny<CancellationToken>()), Times.Once);
    }

    // New tests for uncovered methods

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetInteractionsForInteractionIdAsync_ReturnsEmptyList_WhenIdIsNullOrWhitespace(string id)
    {
        var result = await _service.GetInteractionsForInteractionIdAsync(id);
        Assert.Empty(result);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetInteractionsForInteractionIdAsync_ReturnsEmptyList_WhenNoInteractionsFound()
    {
        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<OMInteraction>());

        var result = await _service.GetInteractionsForInteractionIdAsync("int123");
        Assert.Empty(result);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInteractionsForInteractionIdAsync_ReturnsDtoList_WhenInteractionsExist()
    {
        var interactions = new List<OMInteraction>
        {
            new OMInteraction { Id = "int123", Notes = "InteractionNote", Status = "Active" }
        };

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interactions);

        var result = await _service.GetInteractionsForInteractionIdAsync("int123");

        Assert.Single(result);
        Assert.Equal("InteractionNote", result[0].Notes);
        Assert.Equal("Active", result[0].Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetInteractionsForCaseByCaseReferenceNumberAsync_ReturnsEmptyList_WhenReferenceNullOrWhitespace(string reference)
    {
        var result = await _service.GetInteractionsForCaseByCaseReferenceNumberAsync(reference);
        Assert.Empty(result);
        _mockCaseService.Verify(s => s.GetCasesForCustomerByReferenceNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCaseReferenceNumberAsync_ReturnsEmptyList_WhenNoCasesFound()
    {
        _mockCaseService
            .Setup(s => s.GetCasesForCustomerByReferenceNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OMCaseListResponse());

        var result = await _service.GetInteractionsForCaseByCaseReferenceNumberAsync("ref123");
        Assert.Empty(result);
        _mockCaseService.Verify(s => s.GetCasesForCustomerByReferenceNumberAsync("ref123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCaseReferenceNumberAsync_ReturnsAggregatedInteractions()
    {
        var cases = new OMCaseListResponse
        {
            Data = new List<OMCaseDto>
            {
                new OMCaseDto { Channel = "Web", IdentificationNumber = "custX", Status = "Open", Id = "caseA" },
                new OMCaseDto { Channel = "Phone", IdentificationNumber = "custX", Status = "Closed", Id = "caseB" }
            }
        };

        _mockCaseService
            .Setup(s => s.GetCasesForCustomerByReferenceNumberAsync("refABC", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cases);

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(expr => expr.Compile().Invoke(new OMInteraction { CaseId = "caseA" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { CaseId = "caseA", Notes = "NoteA", Status = "Active" } });

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(expr => expr.Compile().Invoke(new OMInteraction { CaseId = "caseB" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { CaseId = "caseB", Notes = "NoteB", Status = "Closed" } });

        var result = await _service.GetInteractionsForCaseByCaseReferenceNumberAsync("refABC", It.IsAny<CancellationToken>());
        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Notes == "NoteA");
        Assert.Contains(result, i => i.Notes == "NoteB");
        _mockCaseService.Verify(s => s.GetCasesForCustomerByReferenceNumberAsync("refABC", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task InteractionExistsWithReferenceNumberAsync_ReturnsError_WhenReferenceNumberIsNullOrWhitespace(string reference)
    {
        var response = await _service.InteractionExistsWithReferenceNumberAsync(reference);
        Assert.False(response.Success);
        Assert.Contains("Reference number is required.", response.ErrorMessages);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InteractionExistsWithReferenceNumberAsync_ReturnsTrue_WhenInteractionExists()
    {
        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { ReferenceNumber = "ref123" } });

        var response = await _service.InteractionExistsWithReferenceNumberAsync("ref123");

        Assert.True(response.Data);
        Assert.True(response.Success);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InteractionExistsWithReferenceNumberAsync_HandlesRepositoryException()
    {
        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        var response = await _service.InteractionExistsWithReferenceNumberAsync("refError");

        Assert.False(response.Success);
        Assert.NotNull(response.CustomExceptions);
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task InteractionExistsWithIdAsync_ReturnsError_WhenIdIsNullOrWhitespace(string id)
    {
        var response = await _service.InteractionExistsWithIdAsync(id);
        Assert.False(response.Success);
        Assert.Contains("Interaction Id is required.", response.ErrorMessages);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InteractionExistsWithIdAsync_ReturnsTrue_WhenInteractionExists()
    {
        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { Id = "id123" } });

        var response = await _service.InteractionExistsWithIdAsync("id123");

        Assert.True(response.Data);
        Assert.True(response.Success);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InteractionExistsWithIdAsync_HandlesRepositoryException()
    {
        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        var response = await _service.InteractionExistsWithIdAsync("idError");

        Assert.False(response.Success);
        Assert.NotNull(response.CustomExceptions);
        _loggingServiceMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }
}
