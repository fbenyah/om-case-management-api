using FluentValidation.Results;
using om.servicing.casemanagement.application.Services.Models;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Application.Services.Models;

public class BaseItemExistsResponseTests
{
    [Fact]
    public void Constructor_InitializesDataToFalse()
    {
        var response = new TestBaseItemExistsResponse();
        Assert.NotNull(response);
        Assert.False(response.Data);
        Assert.True((DateTime.Now - response.ResponseTime).TotalSeconds < 5);
    }

    [Fact]
    public void Data_CanBeSetAndRetrieved()
    {
        var response = new TestBaseItemExistsResponse { Data = true };
        Assert.True(response.Data);

        response.Data = false;
        Assert.False(response.Data);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorMessageAndSetsSuccessFalse()
    {
        var response = new TestBaseItemExistsResponse();
        response.SetOrUpdateErrorMessage("Some error occurred.");
        Assert.Contains("Some error occurred.", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsMultipleErrorMessages()
    {
        var response = new TestBaseItemExistsResponse();
        var errors = new List<string> { "Error 1", "Error 2" };
        response.SetOrUpdateErrorMessages(errors);
        Assert.Contains("Error 1", response.ErrorMessages);
        Assert.Contains("Error 2", response.ErrorMessages);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsCustomExceptionAndSetsSuccessFalse()
    {
        var response = new TestBaseItemExistsResponse();
        var customException = new ClientException("Custom error");
        response.SetOrUpdateCustomException(customException);
        Assert.NotNull(response.CustomExceptions);
        Assert.Contains(customException, response.CustomExceptions);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetOrUpdateValidationResult_AddsValidationFailuresAndSetsSuccessFalse()
    {
        var response = new TestBaseItemExistsResponse();
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

public class TestBaseItemExistsResponse : BaseItemExistsResponse { }