using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

public class OMTransactionCreateResponse : ServiceBaseResponse<BasicTransactionCreateResponse>
{
    public OMTransactionCreateResponse()
    {
        Data = new();
    }
}

public class BasicTransactionCreateResponse : BaseCreateItemResponse
{
    public string InteractionId { get; set; } = string.Empty;
    public string InteractionReferenceNumber { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string CaseReferenceNumber { get; set; } = string.Empty;
}
