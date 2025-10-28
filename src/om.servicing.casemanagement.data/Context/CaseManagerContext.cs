using Microsoft.EntityFrameworkCore;
using om.servicing.casemanagement.data.Seed;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Utilities;

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

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        RunSeedTransactionTypes(modelBuilder);

        // Runs seed dummy data
        RunSeedDummyData(modelBuilder);
    }

    private void RunSeedTransactionTypes(ModelBuilder modelBuilder)
    {
        // pass fixed date to keep migration deterministic
        var createdDate = new DateTime(2025, 10, 28);
        var transactionTypes = MigrationTransactionTypeSeed.ForMigration(createdDate).ToArray();
        modelBuilder.Entity<OMTransactionType>().HasData(transactionTypes.ToArray());
    }

    private void RunSeedDummyData(ModelBuilder modelBuilder)
    {
        // pass fixed date to keep migration deterministic
        var seed = MigrationDummyData.GetTwoCaseGraph(new DateTime(2025, 10, 28));

        modelBuilder.Entity<OMTransactionType>().HasData(seed.TransactionTypes.ToArray());
        modelBuilder.Entity<OMCase>().HasData(seed.Cases.ToArray());
        modelBuilder.Entity<OMInteraction>().HasData(seed.Interactions.ToArray());
        modelBuilder.Entity<OMTransaction>().HasData(seed.Transactions.ToArray());
    }
}
