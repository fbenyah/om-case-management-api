using OM.RequestFramework.Core.Exceptions;
using static OM.RequestFramework.Core.Exceptions.ExceptionConstants;

namespace om.servicing.casemanagement.domain.Exceptions.Client;

public class ConflictException : ClientException
{
    public override int HttpResponseCode => 409;

    public ConflictException(string message = ErrorMessages.ValidationError) : base(message)
    {
    }
}
