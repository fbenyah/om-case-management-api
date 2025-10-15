using om.servicing.casemanagement.application.Features.OMTransactions.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMTransactions.Queries;

public class GetTransactionsForCaseByCustomerIdentificationQueryTests
{
    [Fact]
    public void Constructor_InitializesCustomerIdentificationNumberToEmptyString()
    {
        var query = new GetTransactionsForCaseByCustomerIdentificationQuery();
        Assert.Equal(string.Empty, query.CustomerIdentificationNumber);
    }

    [Fact]
    public void CustomerIdentificationNumber_CanBeSetAndRetrieved()
    {
        var query = new GetTransactionsForCaseByCustomerIdentificationQuery
        {
            CustomerIdentificationNumber = "CUST123"
        };
        Assert.Equal("CUST123", query.CustomerIdentificationNumber);
    }
}
