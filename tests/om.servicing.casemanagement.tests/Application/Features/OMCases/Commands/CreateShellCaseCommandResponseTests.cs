using om.servicing.casemanagement.application.Features.OMCases.Commands;
using om.servicing.casemanagement.application.Services.Models;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Commands;

public class CreateShellCaseCommandResponseTests
{
    [Fact]
    public void Constructor_InitializesDataToBasicCaseCreateResponse()
    {
        var response = new CreateShellCaseCommandResponse();
        Assert.NotNull(response.Data);
        Assert.IsType<BasicCaseCreateResponse>(response.Data);
        Assert.Empty(response.Data.Id);
        Assert.Empty(response.Data.ReferenceNumber);
    }

    [Fact]
    public void Data_CanBeSetAndRetrieved()
    {
        var basicResponse = new BasicCaseCreateResponse
        {
            Id = "CASE123",
            ReferenceNumber = "REF456"
        };
        var response = new CreateShellCaseCommandResponse
        {
            Data = basicResponse
        };
        Assert.Equal("CASE123", response.Data.Id);
        Assert.Equal("REF456", response.Data.ReferenceNumber);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorMessageAndSetsSuccessFalse()
    {
        var response = new CreateShellCaseCommandResponse();
        response.SetOrUpdateErrorMessage("Some error occurred.");
        Assert.Contains("Some error occurred.", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new CreateShellCaseCommandResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsCustomException()
    {
        var response = new CreateShellCaseCommandResponse();
        var customException = new ClientException("Custom error");
        response.SetOrUpdateCustomException(customException);
        Assert.Contains(customException, response.CustomExceptions);
        Assert.False(response.Success);
    }
}
