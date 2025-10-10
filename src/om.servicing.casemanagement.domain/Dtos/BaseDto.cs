namespace om.servicing.casemanagement.domain.Dtos;

/// <summary>
/// Represents the base class for Data Transfer Objects (DTOs), providing common properties  for identifying and
/// tracking the creation and modification of entities.
/// </summary>
/// <remarks>This class is intended to be inherited by specific DTO types to ensure consistency  in the
/// representation of entity metadata, such as unique identifiers and timestamps.</remarks>
public abstract class BaseDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; } = null;
    public DateTime? UpdateDate { get; set; } = null;
}


/// <summary>
/// Represents a base data transfer object (DTO) that includes a status field.
/// </summary>
/// <remarks>This class is intended to be used as a base for DTOs that require a status property  to represent the
/// state or condition of the object. The <see cref="Status"/> property  is initialized to an empty string by
/// default.</remarks>
public abstract class BaseDtoWithStatus : BaseDto
{
    public string Status { get; set; } = string.Empty;
}
