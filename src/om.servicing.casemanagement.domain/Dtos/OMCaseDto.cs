namespace om.servicing.casemanagement.domain.Dtos;

/// <summary>
/// Represents a case in the Order Management system, including its associated channel and interactions.
/// </summary>
/// <remarks>This data transfer object (DTO) is used to encapsulate information about a case, such as the channel 
/// through which the case was initiated and any related interactions. It is typically used for transferring  case data
/// between different layers of the application.</remarks>
public class OMCaseDto : BaseDtoWithStatus
{
    public string Channel { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;

    public List<OMInteractionDto>? Interactions { get; set; } = null;
}
