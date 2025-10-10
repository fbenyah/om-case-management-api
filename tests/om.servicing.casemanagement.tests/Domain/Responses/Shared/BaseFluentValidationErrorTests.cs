using FluentValidation.Results;
using om.servicing.casemanagement.tests.Shared.Models;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Domain.Responses.Shared;

public class BaseFluentValidationErrorTests
{
    [Fact]
    public void DefaultConstructor_SetsSuccessTrue()
    {
        var error = new TestFluentValidationError();
        Assert.True(error.Success);
        Assert.Empty(error.ErrorMessages);
        Assert.Null(error.CustomExceptions);
    }

    [Fact]
    public void ValidationResultConstructor_SetsErrorMessagesAndSuccessFalse()
    {
        var failures = new List<ValidationFailure>
    {
        new ValidationFailure("Prop", "Error", "Value")
    };
        var result = new ValidationResult(failures);

        var error = new TestFluentValidationError(result);

        Assert.False(error.Success);
        Assert.Single(error.ErrorMessages);
        Assert.Contains("Error on property 'Prop' with value (Value)", error.ErrorMessages);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorAndSetsSuccessFalse()
    {
        var error = new TestFluentValidationError();
        error.SetOrUpdateErrorMessage("Test error");

        Assert.False(error.Success);
        Assert.Contains("Test error", error.ErrorMessages);
    }

    [Fact]
    public void SetOrUpdateErrorMessages_AddsErrorsAndSetsSuccessFalse()
    {
        var error = new TestFluentValidationError();
        error.SetOrUpdateErrorMessages(new List<string> { "Err1", "Err2" });

        Assert.False(error.Success);
        Assert.Contains("Err1", error.ErrorMessages);
        Assert.Contains("Err2", error.ErrorMessages);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsExceptionAndSetsSuccessFalse()
    {
        var error = new TestFluentValidationError();
        var customEx = new DummyCustomException();
        error.SetOrUpdateCustomException(customEx);

        Assert.False(error.Success);
        Assert.Single(error.CustomExceptions);
        Assert.Contains(customEx, error.CustomExceptions);
    }

    [Fact]
    public void SetOrUpdateCustomExceptions_AddsExceptionsAndSetsSuccessFalse()
    {
        var error = new TestFluentValidationError();
        var ex1 = new DummyCustomException();
        var ex2 = new DummyCustomException();
        error.SetOrUpdateCustomExceptions(new List<ICustomException> { ex1, ex2 });

        Assert.False(error.Success);
        Assert.Equal(2, error.CustomExceptions.Count);
    }

    [Fact]
    public void ClearExistingErrors_RemovesPreviousErrors()
    {
        var error = new TestFluentValidationError();
        error.SetOrUpdateErrorMessage("Old error");
        error.SetOrUpdateErrorMessage("New error", clearExistingErrors: true);

        Assert.Single(error.ErrorMessages);
        Assert.Contains("New error", error.ErrorMessages);
    }

    [Fact]
    public void ClearExistingCustomExceptions_RemovesPreviousExceptions()
    {
        var error = new TestFluentValidationError();
        var ex1 = new DummyCustomException();
        var ex2 = new DummyCustomException();
        error.SetOrUpdateCustomException(ex1);
        error.SetOrUpdateCustomException(ex2, clearExistingErrors: true);

        Assert.Single(error.CustomExceptions);
        Assert.Contains(ex2, error.CustomExceptions);
    }
}
