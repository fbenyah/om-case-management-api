namespace om.servicing.casemanagement.domain.Dtos;

/// <summary>
/// Represents a data transfer object (DTO) for a transaction type in the system.
/// </summary>
/// <remarks>This class is used to encapsulate information about a specific transaction type,  including its name,
/// description, and whether it requires approval. It is typically  used for transferring transaction type data between
/// different layers of the application.</remarks>
public class OMTransactionTypeDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiresApproval { get; set; }
}
