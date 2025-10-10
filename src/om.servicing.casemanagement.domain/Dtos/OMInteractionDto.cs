namespace om.servicing.casemanagement.domain.Dtos;

/// <summary>
/// Represents an interaction within the operational management system, including associated notes,  a related case, and
/// a collection of transactions.
/// </summary>
/// <remarks>This data transfer object (DTO) is used to encapsulate information about an interaction,  including
/// optional references to a related case and a list of transactions.  The <see cref="Notes"/> property provides
/// additional context or comments about the interaction.</remarks>
public class OMInteractionDto : BaseDtoWithStatus
{
    public string Notes { get; set; } = string.Empty;
    public OMCaseDto? Case { get; set; } = null;    
    public List<OMTransactionDto>? Transactions { get; set; } = null;   
}
