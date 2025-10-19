using om.servicing.casemanagement.application.Features.OMCases.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByReferenceNumberQueryTests
{
    [Fact]
    public void Constructor_InitializesReferenceNumberToEmptyString()
    {
        var query = new GetCustomerCasesByReferenceNumberQuery();
        Assert.Equal(string.Empty, query.ReferenceNumber);
    }

    [Fact]
    public void ReferenceNumber_CanBeSetAndRetrieved()
    {
        var query = new GetCustomerCasesByReferenceNumberQuery
        {
            ReferenceNumber = "ABC123"
        };
        Assert.Equal("ABC123", query.ReferenceNumber);
    }
}
