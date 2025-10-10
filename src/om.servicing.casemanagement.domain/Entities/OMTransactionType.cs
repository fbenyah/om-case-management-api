using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace om.servicing.casemanagement.domain.Entities;

/// <summary>
/// Represents a type of transaction with associated metadata, including its name, description, and approval
/// requirements.
/// </summary>
/// <remarks>This class is mapped to the "transaction_type" table in the database. It provides details about a
/// specific transaction type, such as its name, a description of its purpose, and whether it requires approval before
/// execution.</remarks>
[Table("transaction_type")]
public class OMTransactionType : BaseEntity
{
    [Required]
    [MaxLength(500)]
    [Column("name")]
    public string Name { get; set; }

    [Required]
    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Required]
    [Column("requires_approval")]
    public bool RequiresApproval { get; set; }
}
