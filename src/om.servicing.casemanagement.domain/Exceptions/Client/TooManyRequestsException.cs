using OM.RequestFramework.Core.Exceptions;
using static OM.RequestFramework.Core.Exceptions.ExceptionConstants;

namespace om.servicing.casemanagement.domain.Exceptions.Client;

public class TooManyRequestsException : ClientException
{
    public override int HttpResponseCode => 429;

    public TooManyRequestsException(string message = ErrorMessages.ValidationError) : base(message)
    {
    }
}
