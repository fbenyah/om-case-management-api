using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

public class OMCaseCreateResponse : ServiceBaseResponse<BasicCaseCreateResponse>
{
    public OMCaseCreateResponse()
    {
        Data = new BasicCaseCreateResponse();
    }
}

public class BasicCaseCreateResponse
{
    public string Id { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
}
