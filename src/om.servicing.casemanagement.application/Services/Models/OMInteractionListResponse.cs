using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

public class OMInteractionListResponse : ServiceBaseResponse<List<domain.Dtos.OMInteractionDto>>
{
    public OMInteractionListResponse()
    {
        Data = new List<domain.Dtos.OMInteractionDto>();
    }
}