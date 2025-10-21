using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

public class OMTransactionListResponse : ServiceBaseResponse<List<domain.Dtos.OMTransactionDto>>
{
    public OMTransactionListResponse()
    {
        Data = new List<domain.Dtos.OMTransactionDto>();
    }
}
