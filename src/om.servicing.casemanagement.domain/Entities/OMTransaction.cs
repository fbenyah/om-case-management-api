using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace om.servicing.casemanagement.domain.Entities;

/// <summary>
/// Represents a transaction entity that is associated with a case, interaction, and transaction type.
/// </summary>
/// <remarks>This class models a transaction within the system, including its relationships to other entities such
/// as cases, interactions, and transaction types. It also includes details about whether the transaction is immediate
/// and information about its received and processed states.</remarks>
[Table("transaction")]
public class OMTransaction : BaseEntityWithReferenceNumberAndStatus
{
    [ForeignKey("case")]
    [MaxLength(50)]
    [Column("case_id", Order = 2)]
    public string CaseId { get; set; }
    public OMCase Case { get; set; }


    [ForeignKey("interaction")]
    [MaxLength(50)]
    [Column("interaction_id", Order = 3)]
    public string InteractionId { get; set; }
    public OMInteraction Interaction { get; set; }


    [ForeignKey("transaction_type")]
    [MaxLength(50)]
    [Column("transaction_type_id", Order = 4)]
    public string TransactionTypeId { get; set; }
    public OMTransactionType TransactionType { get; set; }


    [Required]
    [Column("is_immediate")]
    public bool IsImmediate { get; set; } = false;

    [Required]
    [Column("is_fulfilled_externally")]
    public bool IsFulfilledExternally { get; set; } = false;

    [MaxLength(50)]
    [Column("external_system")]
    public string ExternalSystem { get; set; }

    [MaxLength(50)]
    [Column("external_system_id")]
    public string ExternalSystemId { get; set; }

    [Required]
    [Column("received_details", TypeName = "text")]
    public string ReceivedDetails { get; set; }

    [Column("processed_details", TypeName = "text")]
    public string ProcessedDetails { get; set; }
}
