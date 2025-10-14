using Microsoft.EntityFrameworkCore;
using om.servicing.casemanagement.data.Context;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Data.Context;

public class CaseManagerContextTests
{
    private DbContextOptions<CaseManagerContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<CaseManagerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CanAddAndRetrieveCase()
    {
        var options = CreateInMemoryOptions();
        using (var context = new CaseManagerContext(options))
        {
            var caseEntity = new OMCase
            {
                Id = "case1",
                Channel = "Email",
                IdentificationNumber = "ID123",
                ReferenceNumber = "REF123",
                Status = "Open",
                CreatedDate = DateTime.UtcNow
            };
            context.Cases.Add(caseEntity);
            await context.SaveChangesAsync();
        }

        using (var context = new CaseManagerContext(options))
        {
            var retrieved = context.Cases.SingleOrDefault(c => c.Id == "case1");
            Assert.NotNull(retrieved);
            Assert.Equal("Email", retrieved.Channel);
            Assert.Equal("ID123", retrieved.IdentificationNumber);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveInteraction()
    {
        var options = CreateInMemoryOptions();
        using (var context = new CaseManagerContext(options))
        {
            var caseEntity = new OMCase
            {
                Id = "case2",
                Channel = "Phone",
                IdentificationNumber = "ID456",
                ReferenceNumber = "REF123",
                Status = "Closed",
                CreatedDate = DateTime.UtcNow
            };
            context.Cases.Add(caseEntity);

            var interaction = new OMInteraction
            {
                Id = "int1",
                CaseId = "case2",
                ReferenceNumber = "REF987",
                Status = "Active",
                Notes = "Test notes",
                CreatedDate = DateTime.UtcNow
            };
            context.Interactions.Add(interaction);
            await context.SaveChangesAsync();
        }

        using (var context = new CaseManagerContext(options))
        {
            var retrieved = context.Interactions.SingleOrDefault(i => i.Id == "int1");
            Assert.NotNull(retrieved);
            Assert.Equal("case2", retrieved.CaseId);
            Assert.Equal("Test notes", retrieved.Notes);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveTransaction()
    {
        var options = CreateInMemoryOptions();
        using (var context = new CaseManagerContext(options))
        {
            var caseEntity = new OMCase
            {
                Id = "case3",
                Channel = "Web",
                IdentificationNumber = "ID789",
                ReferenceNumber = "REF123",
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };
            context.Cases.Add(caseEntity);

            var interaction = new OMInteraction
            {
                Id = "int2",
                CaseId = "case3",
                Status = "Active",
                ReferenceNumber = "REF987",
                Notes = "Interaction notes",
                CreatedDate = DateTime.UtcNow
            };
            context.Interactions.Add(interaction);

            var transactionType = new OMTransactionType
            {
                Id = "type1",
                Name = "Payment",
                Description = "Payment transaction",
                RequiresApproval = true,
                CreatedDate = DateTime.UtcNow
            };
            context.TransactionTypes.Add(transactionType);

            var transaction = new OMTransaction
            {
                Id = "txn1",
                CaseId = "case3",
                InteractionId = "int2",
                TransactionTypeId = "type1",
                Status = "Processed",
                ReferenceNumber = "REF654",
                IsImmediate = true,
                ReceivedDetails = "Received",
                ProcessedDetails = "Processed",
                CreatedDate = DateTime.UtcNow
            };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
        }

        using (var context = new CaseManagerContext(options))
        {
            var retrieved = context.Transactions.SingleOrDefault(t => t.Id == "txn1");
            Assert.NotNull(retrieved);
            Assert.Equal("case3", retrieved.CaseId);
            Assert.Equal("int2", retrieved.InteractionId);
            Assert.Equal("type1", retrieved.TransactionTypeId);
            Assert.True(retrieved.IsImmediate);
            Assert.Equal("Received", retrieved.ReceivedDetails);
            Assert.Equal("Processed", retrieved.ProcessedDetails);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveTransactionType()
    {
        var options = CreateInMemoryOptions();
        using (var context = new CaseManagerContext(options))
        {
            var transactionType = new OMTransactionType
            {
                Id = "type2",
                Name = "Refund",
                Description = "Refund transaction",
                RequiresApproval = false,
                CreatedDate = DateTime.UtcNow
            };
            context.TransactionTypes.Add(transactionType);
            await context.SaveChangesAsync();
        }

        using (var context = new CaseManagerContext(options))
        {
            var retrieved = context.TransactionTypes.SingleOrDefault(tt => tt.Id == "type2");
            Assert.NotNull(retrieved);
            Assert.Equal("Refund", retrieved.Name);
            Assert.False(retrieved.RequiresApproval);
        }
    }
}
