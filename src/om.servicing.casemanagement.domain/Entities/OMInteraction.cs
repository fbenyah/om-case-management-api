using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace om.servicing.casemanagement.domain.Entities;

/// <summary>
/// Represents an interaction associated with a specific case, including related notes and transactions.
/// </summary>
/// <remarks>This entity is mapped to the "interaction" table in the database. It contains details about the
/// interaction, such as the associated case, any notes, and a collection of related transactions.</remarks>
[Table("interaction")]
public class OMInteraction : BaseEntityWithReferenceNumberAndStatus
{
    [ForeignKey("case")]
    [MaxLength(50)]
    [Column("case_id", Order = 2)]
    public string CaseId { get; set; }
    public OMCase Case { get; set; }

    [Column("notes", TypeName = "text")]
    public string Notes { get; set; }

    [Required]
    [Column("is_primary_interaction")]
    public bool IsPrimaryInteraction { get; set; } = true;

    [Column("previous_interaction_id")]
    public string PreviousInteractionId { get; set; } = string.Empty;

    public ICollection<OMTransaction> Transactions { get; set; }
}
