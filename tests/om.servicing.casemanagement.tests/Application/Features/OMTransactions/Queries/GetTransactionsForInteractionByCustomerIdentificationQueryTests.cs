using om.servicing.casemanagement.application.Features.OMTransactions.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMTransactions.Queries;

public class GetTransactionsForInteractionByCustomerIdentificationQueryTests
{
    [Fact]
    public void Constructor_InitializesPropertiesToEmptyStrings()
    {
        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery();
        Assert.Equal(string.Empty, query.InteractionId);
        Assert.Equal(string.Empty, query.CustomerIdentificationNumber);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var query = new GetTransactionsForInteractionByCustomerIdentificationQuery
        {
            InteractionId = "INT123",
            CustomerIdentificationNumber = "CUST456"
        };
        Assert.Equal("INT123", query.InteractionId);
        Assert.Equal("CUST456", query.CustomerIdentificationNumber);
    }
}
