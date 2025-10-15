using om.servicing.casemanagement.application.Features.OMTransactions.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMTransactions.Queries;

public class GetTransactionsForCaseByCaseIdQueryTests
{
    [Fact]
    public void Constructor_InitializesCaseIdToEmptyString()
    {
        var query = new GetTransactionsForCaseByCaseIdQuery();
        Assert.Equal(string.Empty, query.CaseId);
    }

    [Fact]
    public void CaseId_CanBeSetAndRetrieved()
    {
        var query = new GetTransactionsForCaseByCaseIdQuery
        {
            CaseId = "CASE123"
        };
        Assert.Equal("CASE123", query.CaseId);
    }
}
