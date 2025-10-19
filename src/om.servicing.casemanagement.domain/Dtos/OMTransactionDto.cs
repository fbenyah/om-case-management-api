namespace om.servicing.casemanagement.domain.Dtos;

/// <summary>
/// Represents a transaction in the Order Management (OM) system, including details about the associated case,
/// interaction, and transaction type.
/// </summary>
/// <remarks>This data transfer object (DTO) is used to encapsulate information about a transaction, including its
/// status, related entities, and processing details. It is typically used to transfer transaction data between
/// different layers of the application.</remarks>
public class OMTransactionDto : BaseDtoWithReferenceNumberAndStatus
{
    public OMCaseDto? Case { get; set; } = null;
    public OMInteractionDto? Interaction { get; set; } = null;
    public OMTransactionTypeDto? TransactionType { get; set; } = null;    
    public bool IsImmediate { get; set; } = false;
    public bool IsFulfilledExternally { get; set; } = false;
    public string ExternalSystem { get; set; } = string.Empty;
    public string ExternalSystemId { get; set; } = string.Empty;
    public string ReceivedDetails { get; set; } = string.Empty;
    public string ProcessedDetails { get; set; } = string.Empty;
}
