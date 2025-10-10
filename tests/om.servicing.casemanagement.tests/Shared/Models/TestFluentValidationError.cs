using FluentValidation.Results;
using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.tests.Shared.Models;

public class TestFluentValidationError : BaseFluentValidationError
{
    public TestFluentValidationError() : base() { }
    public TestFluentValidationError(ValidationResult result) : base(result) { }
}
