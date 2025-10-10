using FluentValidation.Results;
using om.servicing.casemanagement.domain.Responses.Shared;
using om.servicing.casemanagement.tests.Shared.Models;

namespace om.servicing.casemanagement.tests.Domain.Responses.Shared;

public class ApplicationBaseResponseTests
{
    [Fact]
    public void DataProperty_CanSetAndGet()
    {
        var response = new TestApplicationResponse<string>();
        response.Data = "TestData";
        Assert.Equal("TestData", response.Data);
    }

    [Fact]
    public void DefaultConstructor_SetsSuccessTrue()
    {
        var response = new TestApplicationResponse<int>();
        Assert.True(response.Success);
        Assert.Empty(response.ErrorMessages);
        Assert.Null(response.CustomExceptions);
    }

    [Fact]
    public void ValidationResultConstructor_SetsErrorMessagesAndSuccessFalse()
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Prop", "Error", "Value")
        };
        var result = new ValidationResult(failures);

        var response = new TestApplicationResponse<object>();
        response.SetOrUpdateValidationResult(result);

        Assert.False(response.Success);
        Assert.Single(response.ErrorMessages);
        Assert.Contains("Error on property 'Prop' with value (Value)", response.ErrorMessages);
    }

    [Fact]
    public void SetOrUpdateErrorMessage_AddsErrorAndSetsSuccessFalse()
    {
        var response = new TestApplicationResponse<object>();
        response.SetOrUpdateErrorMessage("Test error");

        Assert.False(response.Success);
        Assert.Contains("Test error", response.ErrorMessages);
    }

    [Fact]
    public void SetOrUpdateCustomException_AddsExceptionAndSetsSuccessFalse()
    {
        var response = new TestApplicationResponse<object>();
        var customEx = new DummyCustomException();
        response.SetOrUpdateCustomException(customEx);

        Assert.False(response.Success);
        Assert.Single(response.CustomExceptions);
        Assert.Contains(customEx, response.CustomExceptions);
    }
}

public class TestApplicationResponse<T> : ApplicationBaseResponse<T>
{
    public TestApplicationResponse() : base() { }
}
