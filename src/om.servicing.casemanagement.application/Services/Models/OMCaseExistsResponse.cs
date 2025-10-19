using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

public class OMCaseExistsResponse : ServiceBaseResponse<bool>
{
    public OMCaseExistsResponse()
    {
        Data = false;
    }
}
