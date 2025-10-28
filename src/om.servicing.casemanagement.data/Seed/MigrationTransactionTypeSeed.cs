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
    /// <param name="createdDate">The date and time associated with the creation of the migration data. This parameter is not used in the method
    /// logic but may be relevant for external context.</param>
    /// <returns>A collection of <see cref="OMTransactionType"/> objects, each representing a specific type of transaction with
    /// predefined properties.</returns>
    public static IEnumerable<OMTransactionType> ForMigration(DateTime createdDate)
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

    /// <summary>
    /// Generates a collection of predefined <see cref="OMTransactionType"/> objects for runtime use.
    /// </summary>
    /// <remarks>This method creates a set of transaction types with unique, non-deterministic identifiers 
    /// and predefined properties. It is intended for runtime scenarios where deterministic IDs  are not required. Each
    /// transaction type includes a name, description, and a flag indicating  whether approval is required.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> containing a collection of <see cref="OMTransactionType"/> objects with unique
    /// IDs and predefined attributes.</returns>
    public static IEnumerable<OMTransactionType> ForRuntime()
    {
        var now = DateTime.UtcNow;
        return new[]
        {
           new OMTransactionType
            {
                Id = UlidUtils.NewUlidString(),
                Name = "POCR",
                Description = "A standard transaction that does not require approval/consent and requirements on an identified customer and policies owned by that customer.",
                RequiresApproval = false
            },
            new OMTransactionType
            {
                Id = UlidUtils.NewUlidString(),
                Name = "Policy",
                Description = "A transaction that relates to a policy number.",
                RequiresApproval = true
            },
            new OMTransactionType
            {
                Id = UlidUtils.NewUlidString(),
                Name = "Non-Policy",
                Description = "A transaction that does not relate to a policy number.",
                RequiresApproval = false
            }
        };
    }
}
