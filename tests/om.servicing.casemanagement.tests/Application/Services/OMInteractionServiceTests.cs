using Moq;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Application.Services;

public class OMInteractionServiceTests
{
    private readonly Mock<IOMCaseService> _mockCaseService;
    private readonly Mock<IGenericRepository<OMInteraction>> _mockInteractionRepo;
    private readonly OMInteractionService _service;

    public OMInteractionServiceTests()
    {
        _mockCaseService = new Mock<IOMCaseService>();
        _mockInteractionRepo = new Mock<IGenericRepository<OMInteraction>>();
        _service = new OMInteractionService(_mockCaseService.Object, _mockInteractionRepo.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetInteractionsForCaseByCaseIdAsync_ReturnsEmptyList_WhenCaseIdIsNullOrWhitespace(string caseId)
    {
        var result = await _service.GetInteractionsForCaseByCaseIdAsync(caseId);
        Assert.Empty(result);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCaseIdAsync_ReturnsEmptyList_WhenNoInteractionsFound()
    {
        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<OMInteraction>());

        var result = await _service.GetInteractionsForCaseByCaseIdAsync("case123");
        Assert.Empty(result);
        _mockInteractionRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCaseIdAsync_ReturnsDtoList_WhenInteractionsExist()
    {
        var interactions = new List<OMInteraction>
    {
        new OMInteraction { CaseId = "case123", Notes = "Note1", Status = "Active" }
    };

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>()))
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
        _mockCaseService.Verify(s => s.GetCasesForCustomerByIdentificationNumberAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetInteractionsForCaseByCustomerIdentificationAsync_ReturnsEmptyList_WhenNoCasesFound()
    {
        _mockCaseService
            .Setup(s => s.GetCasesForCustomerByIdentificationNumberAsync(It.IsAny<string>()))
            .ReturnsAsync(new OMCaseListResponse());

        var result = await _service.GetInteractionsForCaseByCustomerIdentificationAsync("cust123");
        Assert.Empty(result);
        _mockCaseService.Verify(s => s.GetCasesForCustomerByIdentificationNumberAsync("cust123"), Times.Once);
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
            .Setup(s => s.GetCasesForCustomerByIdentificationNumberAsync("cust123"))
            .ReturnsAsync(cases);

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(expr => expr.Compile().Invoke(new OMInteraction { CaseId = "case1" }))))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { CaseId = "case1", Notes = "Note1", Status = "Active" } });

        _mockInteractionRepo
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMInteraction, bool>>>(expr => expr.Compile().Invoke(new OMInteraction { CaseId = "case2" }))))
            .ReturnsAsync(new List<OMInteraction> { new OMInteraction { CaseId = "case2", Notes = "Note2", Status = "Closed" } });

        var result = await _service.GetInteractionsForCaseByCustomerIdentificationAsync("cust123");
        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Notes == "Note1");
        Assert.Contains(result, i => i.Notes == "Note2");
        _mockCaseService.Verify(s => s.GetCasesForCustomerByIdentificationNumberAsync("cust123"), Times.Once);
    }
}
