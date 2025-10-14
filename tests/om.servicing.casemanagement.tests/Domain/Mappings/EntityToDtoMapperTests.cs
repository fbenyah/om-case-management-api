using FluentAssertions;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.tests.Domain.Mappings;

public class EntityToDtoMapperTests
{
    [Fact]
    public void ToDto_OMCase_MapsPropertiesAndInteractions()
    {
        var interaction = new OMInteraction
        {
            Id = "int1",
            Notes = "Test notes",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Active",
            ReferenceNumber = "ref9876",
            Transactions = new List<OMTransaction>()
        };

        var entity = new OMCase
        {
            Id = "case1",
            Channel = "Email",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Open",
            ReferenceNumber = "ref1234",
            Interactions = new List<OMInteraction> { interaction }
        };

        var dto = EntityToDtoMapper.ToDto(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Channel.Should().Be(entity.Channel);
        dto.ReferenceNumber.Should().Be(entity.ReferenceNumber);
        dto.Status.Should().Be(entity.Status);
        dto.Interactions.Should().HaveCount(1);
        dto.Interactions[0].Notes.Should().Be(interaction.Notes);
        dto.Interactions[0].ReferenceNumber.Should().Be(interaction.ReferenceNumber);
    }

    [Fact]
    public void ToDto_OMInteraction_MapsPropertiesAndTransactions()
    {
        var transaction = new OMTransaction
        {
            Id = "txn1",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Processed",
            ReferenceNumber = "ref1234",
            IsImmediate = true,
            ReceivedDetails = "Received",
            ProcessedDetails = "Processed",
            TransactionType = null,
            Interaction = null,
            Case = null
        };

        var entity = new OMInteraction
        {
            Id = "int1",
            Notes = "Interaction notes",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Active",
            ReferenceNumber = "ref9876",
            Case = null,
            Transactions = new List<OMTransaction> { transaction }
        };

        var dto = EntityToDtoMapper.ToDto(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Notes.Should().Be(entity.Notes);
        dto.Status.Should().Be(entity.Status);
        dto.ReferenceNumber.Should().Be(entity.ReferenceNumber);
        dto.Transactions.Should().HaveCount(1);
        dto.Transactions[0].Id.Should().Be(transaction.Id);
        dto.Transactions[0].ReferenceNumber.Should().Be(transaction.ReferenceNumber);
    }

    [Fact]
    public void ToDto_OMTransaction_MapsPropertiesAndNestedEntities()
    {
        var transactionType = new OMTransactionType
        {
            Id = "tt1",
            Name = "Type1",
            Description = "Desc",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            RequiresApproval = true
        };

        var entity = new OMTransaction
        {
            Id = "txn1",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Processed",
            ReferenceNumber = "ref1234",
            IsImmediate = false,
            ReceivedDetails = "Received",
            ProcessedDetails = "Processed",
            TransactionType = transactionType,
            Interaction = null,
            Case = null
        };

        var dto = EntityToDtoMapper.ToDto(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Status.Should().Be(entity.Status);
        dto.IsImmediate.Should().Be(entity.IsImmediate);
        dto.ReferenceNumber.Should().Be(entity.ReferenceNumber);
        dto.TransactionType.Should().NotBeNull();
        dto.TransactionType.Name.Should().Be(transactionType.Name);
    }

    [Fact]
    public void ToDto_OMTransactionType_MapsProperties()
    {
        var entity = new OMTransactionType
        {
            Id = "tt1",
            Name = "Type1",
            Description = "Desc",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            RequiresApproval = true
        };

        var dto = EntityToDtoMapper.ToDto(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Name.Should().Be(entity.Name);
        dto.Description.Should().Be(entity.Description);
        dto.RequiresApproval.Should().Be(entity.RequiresApproval);
    }
}
