using om.servicing.casemanagement.application.Features.OMCases.Queries;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberAndStatusQueryTests
{
    [Fact]
    public void Constructor_InitializesPropertiesToEmptyStrings()
    {
        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery();
        Assert.Equal(string.Empty, query.Status);
        Assert.Equal(string.Empty, query.IdentificationNumber);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var query = new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            Status = "Open",
            IdentificationNumber = "123456"
        };
        Assert.Equal("Open", query.Status);
        Assert.Equal("123456", query.IdentificationNumber);
    }
}
