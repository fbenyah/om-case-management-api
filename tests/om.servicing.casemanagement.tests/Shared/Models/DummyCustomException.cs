using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Shared.Models;

public class DummyCustomException : ICustomException
{
    int ICustomException.HttpResponseCode => throw new NotImplementedException();

    IErrorResponse ICustomException.ToErrorResponse()
    {
        throw new NotImplementedException();
    }
}
