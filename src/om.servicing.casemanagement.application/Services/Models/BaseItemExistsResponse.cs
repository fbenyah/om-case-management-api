using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Services.Models;

/// <summary>
/// Represents the base response for determining whether an item exists.
/// </summary>
/// <remarks>This class provides a boolean response indicating the existence of an item.  Derived classes can
/// extend this functionality to include additional context or metadata.</remarks>
public abstract class BaseItemExistsResponse : ServiceBaseResponse<bool>
{
    public BaseItemExistsResponse()
    {
        Data = false;
    }
}
