using Microsoft.EntityFrameworkCore;
using om.servicing.casemanagement.data.Context;

namespace om.servicing.casemanagement.data.Seed.Runtime;

public static class TransactionTypeRuntimeSeeder
{
    public static async Task EnsureTransactionTypesAsync(CaseManagerContext context)
    {
        // simple idempotent check by Name (choose a key that makes sense)
        var existingNames = await context.TransactionTypes.Select(t => t.Name).ToListAsync();

        var toAdd = MigrationTransactionTypeSeed.ForMigration()
            .Where(t => !existingNames.Contains(t.Name, System.StringComparer.OrdinalIgnoreCase))
            .ToArray();

        if (toAdd.Any())
        {
            context.TransactionTypes.AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }
}
