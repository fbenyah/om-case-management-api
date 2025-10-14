using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace om.servicing.casemanagement.domain.Entities;

/// <summary>
/// Represents the base class for entities with common properties such as an identifier, creation date, and optional
/// update date.
/// </summary>
/// <remarks>This class is intended to be inherited by other entity classes to provide a consistent structure for
/// entity identification and tracking of creation and update timestamps. The <see cref="Id"/> property serves as the
/// unique identifier for the entity, while <see cref="CreatedDate"/> and <see cref="UpdateDate"/> track the entity's
/// lifecycle.</remarks>
public abstract class BaseEntity
{
    [Key]
    [MaxLength(50)]
    [Column("id", Order = 1)]
    public string Id { get; set; }

    [Required]
    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents an entity with a status field, extending the base entity functionality.
/// </summary>
/// <remarks>The <see cref="Status"/> property is required and has a maximum length of 70 characters. This class
/// is intended to be used as a base class for entities that need to include a status field.</remarks>
public abstract class BaseEntityWithStatus : BaseEntity
{
    [Required]
    [MaxLength(70)]
    [Column("status")]
    public string Status { get; set; }
}

/// <summary>
/// Represents an abstract base entity that includes a reference number and a status.
/// </summary>
/// <remarks>This class extends <see cref="BaseEntityWithStatus"/> by adding a required reference number property.
/// The <see cref="ReferenceNumber"/> property is constrained to a maximum length of 20 characters.</remarks>
public abstract class BaseEntityWithReferenceNumberAndStatus : BaseEntityWithStatus
{
    [Required]
    [MaxLength(20)]
    [Column("reference_number")]
    public string ReferenceNumber { get; set; }
}
