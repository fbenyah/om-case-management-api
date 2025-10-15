using om.servicing.casemanagement.application.Features.OMInteractions.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMInteractions.Queries;

public class GetInteractionsForCaseByCustomerIdentificationQueryTests
{
    [Fact]
    public void Constructor_InitializesCustomerIdentificationNumberToEmptyString()
    {
        var query = new GetInteractionsForCaseByCustomerIdentificationQuery();
        Assert.Equal(string.Empty, query.CustomerIdentificationNumber);
    }

    [Fact]
    public void CustomerIdentificationNumber_CanBeSetAndRetrieved()
    {
        var query = new GetInteractionsForCaseByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = "CUST123"
        };
        Assert.Equal("CUST123", query.CustomerIdentificationNumber);
    }
}
