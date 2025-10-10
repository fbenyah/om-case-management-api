using FluentValidation.Results;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.domain.Responses.Shared;

public abstract class BaseFluentValidationError
{
    public bool Success { get; private set; }
    public List<string> ErrorMessages { get; private set; }
    public List<ICustomException>? CustomExceptions { get; private set; }

    public BaseFluentValidationError()
    {
        ErrorMessages = new List<string>();
        Success = NoErrorMessagesAndCustomExceptions();
    }

    public BaseFluentValidationError(ValidationResult validationResult)
    {
        ErrorMessages = ProcessValidationFailures(validationResult);
        Success = NoErrorMessagesAndCustomExceptions();
    }

    public virtual void SetOrUpdateValidationResult(ValidationResult validationResult, bool clearExistingErrors = false)
    {
        if (clearExistingErrors)
            ErrorMessages.Clear();

        ErrorMessages.AddRange(ProcessValidationFailures(validationResult));
        Success = NoErrorMessagesAndCustomExceptions();
    }

    public virtual void SetOrUpdateErrorMessage(string errorMessage, bool clearExistingErrors = false)
    {
        if (clearExistingErrors)
            ErrorMessages.Clear();

        ErrorMessages.Add(errorMessage);
        Success = NoErrorMessagesAndCustomExceptions();
    }

    public virtual void SetOrUpdateErrorMessages(List<string> errorMessages, bool clearExistingErrors = false)
    {
        if (clearExistingErrors)
            ErrorMessages.Clear();

        ErrorMessages.AddRange(errorMessages);
        Success = NoErrorMessagesAndCustomExceptions();
    }

    public virtual void SetOrUpdateCustomException(ICustomException customException, bool clearExistingErrors = false)
    {
        if (clearExistingErrors)
            if (CustomExceptions != null)
                CustomExceptions.Clear();

        if (CustomExceptions == null)
            CustomExceptions = new List<ICustomException>();

        CustomExceptions.Add(customException);
        Success = NoErrorMessagesAndCustomExceptions();
    }

    public virtual void SetOrUpdateCustomExceptions(List<ICustomException> customExceptions, bool clearExistingErrors = false)
    {
        if (clearExistingErrors)
            if (CustomExceptions != null)
                CustomExceptions.Clear();

        if (CustomExceptions == null)
            CustomExceptions = new List<ICustomException>();

        CustomExceptions.AddRange(customExceptions);
        Success = NoErrorMessagesAndCustomExceptions();
    }

    private bool NoErrorMessagesAndCustomExceptions()
    {
        bool everythingSuccessful = NoErrorMessages() && NoCustomExceptions();

        if (everythingSuccessful)
        {
            ErrorMessages.Clear();
            CustomExceptions = null;
        }

        return everythingSuccessful;
    }

    private bool NoErrorMessages()
    {
        return ErrorMessages.Count == 0;
    }

    private bool NoCustomExceptions()
    {
        return CustomExceptions == null
                || (CustomExceptions != null && CustomExceptions.Count == 0);
    }

    private List<string> ProcessValidationFailures(ValidationResult validationResult)
    {
        return
            validationResult != null ?
                        validationResult.Errors.Select(x => $"{x.ErrorMessage} on property '{x.PropertyName}' with value ({x.AttemptedValue})").ToList()
                        : new List<string>();
    }
}
