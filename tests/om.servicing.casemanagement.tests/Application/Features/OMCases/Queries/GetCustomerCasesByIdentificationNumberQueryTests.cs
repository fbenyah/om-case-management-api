using om.servicing.casemanagement.application.Features.OMCases.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberQueryTests
{
    [Fact]
    public void Constructor_InitializesIdentificationNumberToEmptyString()
    {
        var query = new GetCustomerCasesByIdentificationNumberQuery();
        Assert.Equal(string.Empty, query.IdentificationNumber);
    }

    [Fact]
    public void IdentificationNumber_CanBeSetAndRetrieved()
    {
        var query = new GetCustomerCasesByIdentificationNumberQuery
        {
            IdentificationNumber = "ABC123"
        };
        Assert.Equal("ABC123", query.IdentificationNumber);
    }
}
