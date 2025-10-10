using Microsoft.EntityFrameworkCore;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.data.Context;

/// <summary>
/// Represents the database context for managing case-related data, including cases, interactions, transactions, and
/// transaction types.
/// </summary>
/// <remarks>This context is used to interact with the underlying database for operations related to case
/// management.  It provides access to the <see cref="Cases"/>, <see cref="Interactions"/>, <see cref="Transactions"/>,
/// and <see cref="TransactionTypes"/> DbSets,  which correspond to the respective entities in the database.</remarks>
public class CaseManagerContext : DbContext
{
    public CaseManagerContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

    public DbSet<OMCase> Cases { get; set; }
    public DbSet<OMInteraction> Interactions { get; set; }
    public DbSet<OMTransaction> Transactions { get; set; }
    public DbSet<OMTransactionType> TransactionTypes { get; set; }
}
