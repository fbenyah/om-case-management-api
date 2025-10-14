using FluentValidation.Results;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Domain.Responses.Shared;

public class ServiceBaseResponseTests
{
    [Fact]
    public void Constructor_InitializesResponseTimeAndData()
    {
        var response = new TestServiceBaseResponse();
        Assert.True((DateTime.Now - response.ResponseTime).TotalSeconds < 2);
        Assert.Null(response.Data);
    }

    [Fact]
    public void Data_CanBeSetAndRetrieved()
    {
        var response = new TestServiceBaseResponse { Data = "TestData" };
        Assert.Equal("TestData", response.Data);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorMessageAndSetsSuccessFalse()
    {
        var response = new TestServiceBaseResponse();
        response.SetOrUpdateErrorMessage("Some error occurred.");
        Assert.Contains("Some error occurred.", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new TestServiceBaseResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsCustomException()
    {
        var response = new TestServiceBaseResponse();
        var customException = new ClientException("Custom error");

        response.SetOrUpdateCustomException(customException);
        Assert.Contains(customException, response.CustomExceptions);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateValidationResult_AddsValidationFailures()
    {
        var response = new TestServiceBaseResponse();
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Prop", "Validation error")
        };
        var validationResult = new ValidationResult(failures);
        response.SetOrUpdateValidationResult(validationResult);
        Assert.Contains("Validation error on property 'Prop' with value ()", response.ErrorMessages);
        Assert.False(response.Success);
    }
}

// Simple concrete subclass for testing
public class TestServiceBaseResponse : ServiceBaseResponse<string> { }
