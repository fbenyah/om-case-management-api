using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberAndStatusResponseTests
{
    [Fact]
    public void Constructor_InitializesDataAsEmptyList()
    {
        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse();
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
    }

    [Fact]
    public void Data_CanBeSetAndRetrieved()
    {
        var cases = new List<OMCaseDto>
        {
            new OMCaseDto { IdentificationNumber = "123", Status = "Open", Channel = "Web" }
        };

        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse
        {
            Data = cases
        };

        Assert.Equal(cases, response.Data);
        Assert.Single(response.Data);
        Assert.Equal("123", response.Data[0].IdentificationNumber);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_SetsErrorMessageAndSuccessFalse()
    {
        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse();
        response.SetOrUpdateErrorMessage("Test error");

        Assert.Contains("Test error", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void Success_IsTrueByDefault()
    {
        var response = new GetCustomerCasesByIdentificationNumberAndStatusResponse();
        Assert.True(response.Success);
    }
}
