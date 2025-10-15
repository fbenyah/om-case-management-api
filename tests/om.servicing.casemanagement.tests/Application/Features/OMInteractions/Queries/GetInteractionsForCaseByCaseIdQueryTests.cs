using om.servicing.casemanagement.application.Features.OMInteractions.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMInteractions.Queries;

public class GetInteractionsForCaseByCaseIdQueryTests
{
    [Fact]
    public void Constructor_InitializesCaseIdToEmptyString()
    {
        var query = new GetInteractionsForCaseByCaseIdQuery();
        Assert.Equal(string.Empty, query.CaseId);
    }

    [Fact]
    public void CaseId_CanBeSetAndRetrieved()
    {
        var query = new GetInteractionsForCaseByCaseIdQuery
        {
            CaseId = "CASE123"
        };
        Assert.Equal("CASE123", query.CaseId);
    }
}
