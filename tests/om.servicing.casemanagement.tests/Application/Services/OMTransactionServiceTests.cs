using Moq;
using om.servicing.casemanagement.application.Services;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Application.Services;

public class OMTransactionServiceTests
{
    private readonly Mock<IOMCaseService> _caseServiceMock;
    private readonly Mock<IGenericRepository<OMTransaction>> _transactionRepoMock;
    private readonly OMTransactionService _service;

    public OMTransactionServiceTests()
    {
        _caseServiceMock = new Mock<IOMCaseService>();
        _transactionRepoMock = new Mock<IGenericRepository<OMTransaction>>();
        _service = new OMTransactionService(_caseServiceMock.Object, _transactionRepoMock.Object);
    }

    [Fact]
    public async Task GetTransactionsForCaseByCaseIdAsync_NullOrWhitespace_ReturnsEmptyList()
    {
        var result1 = await _service.GetTransactionsForCaseByCaseIdAsync(null);
        var result2 = await _service.GetTransactionsForCaseByCaseIdAsync("");
        var result3 = await _service.GetTransactionsForCaseByCaseIdAsync("   ");
        Assert.Empty(result1);
        Assert.Empty(result2);
        Assert.Empty(result3);
    }

    [Fact]
    public async Task GetTransactionsForCaseByCaseIdAsync_NoTransactions_ReturnsEmptyList()
    {
        _transactionRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMTransaction, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<OMTransaction>());
        var result = await _service.GetTransactionsForCaseByCaseIdAsync("case123");
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTransactionsForCaseByCaseIdAsync_TransactionsExist_ReturnsDtos()
    {
        var transactions = new List<OMTransaction>
    {
        new OMTransaction { CaseId = "case123", ReceivedDetails = "R", ProcessedDetails = "P", IsImmediate = true, Status = "Active" }
    };
        _transactionRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMTransaction, bool>>>()))
            .ReturnsAsync(transactions);

        var result = await _service.GetTransactionsForCaseByCaseIdAsync("case123");
        Assert.Single(result);
        Assert.Equal("R", result[0].ReceivedDetails);
        Assert.Equal("P", result[0].ProcessedDetails);
        Assert.True(result[0].IsImmediate);
        Assert.Equal("Active", result[0].Status);
    }

    [Fact]
    public async Task GetTransactionsForCaseByCustomerIdentificationAsync_NullOrWhitespace_ReturnsEmptyList()
    {
        var result1 = await _service.GetTransactionsForCaseByCustomerIdentificationAsync(null);
        var result2 = await _service.GetTransactionsForCaseByCustomerIdentificationAsync("");
        var result3 = await _service.GetTransactionsForCaseByCustomerIdentificationAsync("   ");
        Assert.Empty(result1);
        Assert.Empty(result2);
        Assert.Empty(result3);
    }

    [Fact]
    public async Task GetTransactionsForCaseByCustomerIdentificationAsync_NoCases_ReturnsEmptyList()
    {
        _caseServiceMock.Setup(s => s.GetCasesForCustomer(It.IsAny<string>()))
            .ReturnsAsync(new List<OMCaseDto>());
        var result = await _service.GetTransactionsForCaseByCustomerIdentificationAsync("cust123");
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTransactionsForCaseByCustomerIdentificationAsync_CasesWithNoTransactions_ReturnsEmptyList()
    {
        _caseServiceMock.Setup(s => s.GetCasesForCustomer(It.IsAny<string>()))
            .ReturnsAsync(new List<OMCaseDto> { new OMCaseDto { Channel = "Web", IdentificationNumber = "cust123" } });
        _transactionRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OMTransaction, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<OMTransaction>());
        var result = await _service.GetTransactionsForCaseByCustomerIdentificationAsync("cust123");
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTransactionsForCaseByCustomerIdentificationAsync_CasesWithTransactions_ReturnsAllDtos()
    {
        var cases = new List<OMCaseDto>
        {
            new OMCaseDto { Channel = "Web", IdentificationNumber = "cust123", Status = "Open", Id = "case1" },
            new OMCaseDto { Channel = "Phone", IdentificationNumber = "cust123", Status = "Closed", Id = "case2" }
        };
        _caseServiceMock.Setup(s => s.GetCasesForCustomer("cust123")).ReturnsAsync(cases);

        _transactionRepoMock
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMTransaction, bool>>>(expr => expr.Compile().Invoke(new OMTransaction { CaseId = "case1" }))))
            .ReturnsAsync(new List<OMTransaction> { new OMTransaction { CaseId = "case1", ReceivedDetails = "R1", ProcessedDetails = "P1", IsImmediate = true, Status = "Active" } });

        _transactionRepoMock
            .Setup(r => r.FindAsync(It.Is<System.Linq.Expressions.Expression<System.Func<OMTransaction, bool>>>(expr => expr.Compile().Invoke(new OMTransaction { CaseId = "case2" }))))
            .ReturnsAsync(new List<OMTransaction> { new OMTransaction { CaseId = "case2", ReceivedDetails = "R2", ProcessedDetails = "P2", IsImmediate = false, Status = "Inactive" } });

        var result = await _service.GetTransactionsForCaseByCustomerIdentificationAsync("cust123");
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.ReceivedDetails == "R1" && t.ProcessedDetails == "P1");
        Assert.Contains(result, t => t.ReceivedDetails == "R2" && t.ProcessedDetails == "P2");
        _caseServiceMock.Verify(s => s.GetCasesForCustomer("cust123"), Times.Once);
    }
}
