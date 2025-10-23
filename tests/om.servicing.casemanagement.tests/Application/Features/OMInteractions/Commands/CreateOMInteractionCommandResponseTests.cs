using om.servicing.casemanagement.application.Features.OMInteractions.Commands;
using om.servicing.casemanagement.application.Services.Models;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Application.Features.OMInteractions.Commands;

public class CreateOMInteractionCommandResponseTests
{
    [Fact]
    public void Constructor_InitializesDataToBasicInteractionCreateResponse()
    {
        var response = new CreateOMInteractionCommandResponse();
        Assert.NotNull(response.Data);
        Assert.IsType<BasicInteractionCreateResponse>(response.Data);
        Assert.Empty(response.Data.Id);
        Assert.Empty(response.Data.ReferenceNumber);
    }

    [Fact]
    public void Data_CanBeSetAndRetrieved()
    {
        var basicResponse = new BasicInteractionCreateResponse
        {
            Id = "INT123",
            ReferenceNumber = "REFINT"
        };
        var response = new CreateOMInteractionCommandResponse
        {
            Data = basicResponse
        };
        Assert.Equal("INT123", response.Data.Id);
        Assert.Equal("REFINT", response.Data.ReferenceNumber);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorMessageAndSetsSuccessFalse()
    {
        var response = new CreateOMInteractionCommandResponse();
        response.SetOrUpdateErrorMessage("Some error occurred.");
        Assert.Contains("Some error occurred.", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new CreateOMInteractionCommandResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsCustomException()
    {
        var response = new CreateOMInteractionCommandResponse();
        var customException = new ClientException("Custom error");
        response.SetOrUpdateCustomException(customException);
        Assert.Contains(customException, response.CustomExceptions);
        Assert.False(response.Success);
    }
}
