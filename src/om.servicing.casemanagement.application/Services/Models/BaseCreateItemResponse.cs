namespace om.servicing.casemanagement.application.Services.Models;

public abstract class BaseCreateItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
}
