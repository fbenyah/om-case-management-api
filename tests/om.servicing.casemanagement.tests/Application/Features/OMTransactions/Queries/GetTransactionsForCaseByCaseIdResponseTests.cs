using om.servicing.casemanagement.application.Features.OMTransactions.Queries;
using om.servicing.casemanagement.domain.Dtos;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Application.Features.OMTransactions.Queries;

public class GetTransactionsForCaseByCaseIdResponseTests
{
    [Fact]
    public void Constructor_InitializesDataToEmptyList()
    {
        var response = new GetTransactionsForCaseByCaseIdResponse();
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
    }

    [Fact]
    public void Data_CanBeSetAndRetrieved()
    {
        var transactions = new List<OMTransactionDto>
    {
        new OMTransactionDto { ReceivedDetails = "Received", ProcessedDetails = "Processed", Status = "Completed" }
    };
        var response = new GetTransactionsForCaseByCaseIdResponse
        {
            Data = transactions
        };
        Assert.Equal(transactions, response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Received", response.Data[0].ReceivedDetails);
        Assert.Equal("Processed", response.Data[0].ProcessedDetails);
        Assert.Equal("Completed", response.Data[0].Status);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorMessageAndSetsSuccessFalse()
    {
        var response = new GetTransactionsForCaseByCaseIdResponse();
        response.SetOrUpdateErrorMessage("Some error occurred.");
        Assert.Contains("Some error occurred.", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new GetTransactionsForCaseByCaseIdResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsCustomException()
    {
        var response = new GetTransactionsForCaseByCaseIdResponse();
        var customException = new ClientException("Custom error");

        response.SetOrUpdateCustomException(customException);
        Assert.Contains(customException, response.CustomExceptions);
        Assert.False(response.Success);
    }
}
