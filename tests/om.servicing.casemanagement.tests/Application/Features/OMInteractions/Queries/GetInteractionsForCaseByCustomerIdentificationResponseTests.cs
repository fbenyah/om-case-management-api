using om.servicing.casemanagement.application.Features.OMInteractions.Queries;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Application.Features.OMInteractions.Queries;

public class GetInteractionsForCaseByCustomerIdentificationResponseTests
{
    [Fact]
    public void Constructor_InitializesDataToEmptyList()
    {
        var response = new GetInteractionsForCaseByCustomerIdentificationResponse();
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
    }

    [Fact]
    public void Data_CanBeSetAndRetrieved()
    {
        var interactions = new List<OMInteractionDto>
        {
            new OMInteractionDto { Notes = "Test", Status = "Active" }
        };
        var response = new GetInteractionsForCaseByCustomerIdentificationResponse
        {
            Data = interactions
        };
        Assert.Equal(interactions, response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Test", response.Data[0].Notes);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorMessageAndSetsSuccessFalse()
    {
        var response = new GetInteractionsForCaseByCustomerIdentificationResponse();
        response.SetOrUpdateErrorMessage("Some error occurred.");
        Assert.Contains("Some error occurred.", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new GetInteractionsForCaseByCustomerIdentificationResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsCustomException()
    {
        var response = new GetInteractionsForCaseByCustomerIdentificationResponse();
        var clientException = new ClientException("Custom error");

        response.SetOrUpdateCustomException(clientException);
        Assert.Contains(clientException, response.CustomExceptions);
        Assert.False(response.Success);
    }
}
