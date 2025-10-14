using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

public class OMCaseListResponse : ServiceBaseResponse<List<domain.Dtos.OMCaseDto>>
{
    public OMCaseListResponse()
    {
        Data = new List<domain.Dtos.OMCaseDto>();
    }
}
