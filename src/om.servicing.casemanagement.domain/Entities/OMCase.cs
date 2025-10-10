using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace om.servicing.casemanagement.domain.Entities;

/// <summary>
/// Represents a case entity, including its associated channel and interactions.
/// </summary>
/// <remarks>This class is mapped to the "case" table in the database. It includes details about the channel 
/// through which the case was created and a collection of related interactions.</remarks>
[Table("case")]
public class OMCase : BaseEntityWithStatus
{
    [Required]
    [MaxLength(70)]
    [Column("channel", Order = 2)]
    public string Channel { get; set; }

    [MaxLength(20)]
    [Column("identificationNumber", Order = 3)]
    public string IdentificationNumber { get; set; }

    public ICollection<OMInteraction> Interactions { get; set; }
}
