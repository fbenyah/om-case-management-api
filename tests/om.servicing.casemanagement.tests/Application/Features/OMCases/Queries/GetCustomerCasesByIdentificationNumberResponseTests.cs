using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberResponseTests
{
    [Fact]
    public void Constructor_InitializesDataToEmptyList()
    {
        var response = new GetCustomerCasesByIdentificationNumberResponse();
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
    }

    [Fact]
    public void CanAssignData()
    {
        var cases = new List<OMCaseDto>
        {
            new OMCaseDto { Channel = "Email", ReferenceNumber = "ref1234", IdentificationNumber = "123", Status = "Open" }
        };
        var response = new GetCustomerCasesByIdentificationNumberResponse
        {
            Data = cases
        };
        Assert.Equal(cases, response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Email", response.Data[0].Channel);
        Assert.Equal("ref1234", response.Data[0].ReferenceNumber);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorMessageAndSetsSuccessFalse()
    {
        var response = new GetCustomerCasesByIdentificationNumberResponse();
        response.SetOrUpdateErrorMessage("Test error");
        Assert.Contains("Test error", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new GetCustomerCasesByIdentificationNumberResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void Success_IsTrueByDefault()
    {
        var response = new GetCustomerCasesByIdentificationNumberResponse();
        Assert.True(response.Success);
    }
}
