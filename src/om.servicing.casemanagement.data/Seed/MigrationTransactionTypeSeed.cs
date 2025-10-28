using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Utilities;

namespace om.servicing.casemanagement.data.Seed;

public class MigrationTransactionTypeSeed
{
    /// <summary>
    /// Generates a deterministic collection of predefined <see cref="OMTransactionType"/> objects for use in database
    /// migrations.
    /// </summary>
    /// <remarks>This method is intended to provide a fixed set of transaction types with unique identifiers,
    /// ensuring consistency and determinism in migration scripts. The generated identifiers are created using <see
    /// cref="UlidUtils.NewUlidString"/> to guarantee uniqueness.</remarks>
    /// logic but may be relevant for external context.</param>
    /// <returns>A collection of <see cref="OMTransactionType"/> objects, each representing a specific type of transaction with
    /// predefined properties.</returns>
    public static IEnumerable<OMTransactionType> ForMigration()
    {
        return new[]
        {
            new OMTransactionType
            {
                Id = "01JFJ0R4E4MTHQ4KSNVQ5H1K3W",
                Name = "POCR",
                Description = "A standard transaction that does not require approval/consent and requirements on an identified customer and policies owned by that customer.",
                RequiresApproval = false
            },
            new OMTransactionType
            {
                Id = "01JFJ0R4E5SK1Q7HBS9D5RX2CP",
                Name = "Policy",
                Description = "A transaction that relates to a policy number.",
                RequiresApproval = true
            },
            new OMTransactionType
            {
                Id = "01JFJ0R4E6G2EHFQ89CD3C9Z2Z",
                Name = "Non-Policy",
                Description = "A transaction that does not relate to a policy number.",
                RequiresApproval = false
            }
        };
    }
}
