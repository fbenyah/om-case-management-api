using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

public class OMInteractionCreateResponse : ServiceBaseResponse<BasicInteractionCreateResponse>
{
    public OMInteractionCreateResponse()
    {
        Data = new();
    }
}

public class BasicInteractionCreateResponse : BaseCreateItemResponse
{
    public string CaseId { get; set; } = string.Empty;
    public string CaseReferenceNumber { get; set; } = string.Empty;
}
