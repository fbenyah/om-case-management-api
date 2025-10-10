using Microsoft.EntityFrameworkCore;
using om.servicing.casemanagement.data.Context;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Data.Context;

public class CaseManagerContextTests
{
    private DbContextOptions<CaseManagerContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<CaseManagerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CanAddAndRetrieveCase()
    {
        var options = CreateOptions();
        var caseEntity = new OMCase
        {
            Id = "case1",
            Channel = "Email",
            Status = "Open",
            CreatedDate = DateTime.UtcNow
        };

        using (var context = new CaseManagerContext(options))
        {
            context.Cases.Add(caseEntity);
            await context.SaveChangesAsync();
        }

        using (var context = new CaseManagerContext(options))
        {
            var retrieved = await context.Cases.FindAsync("case1");
            Assert.NotNull(retrieved);
            Assert.Equal("Email", retrieved.Channel);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveInteractionWithCase()
    {
        var options = CreateOptions();
        var caseEntity = new OMCase
        {
            Id = "case2",
            Channel = "Web",
            Status = "Closed",
            CreatedDate = DateTime.UtcNow
        };
        var interaction = new OMInteraction
        {
            Id = "int1",
            CaseId = "case2",
            Status = "Completed",
            CreatedDate = DateTime.UtcNow,
            Notes = "Interaction notes"
        };

        using (var context = new CaseManagerContext(options))
        {
            context.Cases.Add(caseEntity);
            context.Interactions.Add(interaction);
            await context.SaveChangesAsync();
        }

        using (var context = new CaseManagerContext(options))
        {
            var retrieved = await context.Interactions
                .Include(i => i.Case)
                .FirstOrDefaultAsync(i => i.Id == "int1");
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.Case);
            Assert.Equal("Web", retrieved.Case.Channel);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveTransactionWithRelations()
    {
        var options = CreateOptions();
        var caseEntity = new OMCase
        {
            Id = "case3",
            Channel = "Phone",
            Status = "Open",
            CreatedDate = DateTime.UtcNow
        };
        var interaction = new OMInteraction
        {
            Id = "int2",
            CaseId = "case3",
            Status = "Pending",
            CreatedDate = DateTime.UtcNow,
            Notes = "Interaction for transaction"
        };
        var transactionType = new OMTransactionType
        {
            Id = "type1",
            Name = "Payment",
            Description = "Payment transaction",
            RequiresApproval = true,
            CreatedDate = DateTime.UtcNow
        };
        var transaction = new OMTransaction
        {
            Id = "txn1",
            CaseId = "case3",
            InteractionId = "int2",
            TransactionTypeId = "type1",
            Status = "Processed",
            CreatedDate = DateTime.UtcNow,
            IsImmediate = true,
            ReceivedDetails = "Received",
            ProcessedDetails = "Processed"
        };

        using (var context = new CaseManagerContext(options))
        {
            context.Cases.Add(caseEntity);
            context.Interactions.Add(interaction);
            context.TransactionTypes.Add(transactionType);
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
        }

        using (var context = new CaseManagerContext(options))
        {
            var retrieved = await context.Transactions
                .Include(t => t.Case)
                .Include(t => t.Interaction)
                .Include(t => t.TransactionType)
                .FirstOrDefaultAsync(t => t.Id == "txn1");
            Assert.NotNull(retrieved);
            Assert.Equal("Phone", retrieved.Case.Channel);
            Assert.Equal("Pending", retrieved.Interaction.Status);
            Assert.Equal("Payment", retrieved.TransactionType.Name);
        }
    }
}
