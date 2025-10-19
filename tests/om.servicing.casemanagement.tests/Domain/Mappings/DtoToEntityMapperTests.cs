using FluentAssertions;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.tests.Domain.Mappings;

public class DtoToEntityMapperTests
{
    [Fact]
    public void ToEntity_OMCaseDto_MapsPropertiesAndInteractions()
    {
        var interactionDto = new OMInteractionDto
        {
            Id = "int1",
            Notes = "Test notes",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Active",
            ReferenceNumber = "ref6879",
            Transactions = new List<OMTransactionDto>()
        };

        var dto = new OMCaseDto
        {
            Id = "case1",
            Channel = "Email",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Open",
            ReferenceNumber = "ref1234",
            Interactions = new List<OMInteractionDto> { interactionDto }
        };

        var entity = DtoToEntityMapper.ToEntity(dto);

        entity.Id.Should().Be(dto.Id);
        entity.Channel.Should().Be(dto.Channel);
        entity.Status.Should().Be(dto.Status);
        entity.ReferenceNumber.Should().Be(dto.ReferenceNumber);
        entity.Interactions.Should().HaveCount(1);
        entity.Interactions.Should().AllBeOfType<OMInteraction>();
        entity.Interactions.First().Notes.Should().Be(interactionDto.Notes);
        entity.Interactions.First().ReferenceNumber.Should().Be(interactionDto.ReferenceNumber);
    }

    [Fact]
    public void ToEntity_OMCaseDto_NullInteractions_InitializesEmptyList()
    {
        var dto = new OMCaseDto
        {
            Id = "case2",
            Channel = "Web",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Closed",
            ReferenceNumber = "ref1234",
            Interactions = null
        };

        var entity = DtoToEntityMapper.ToEntity(dto);

        entity.Interactions.Should().NotBeNull();
        entity.Interactions.Should().BeEmpty();
    }

    [Fact]
    public void ToEntity_OMInteractionDto_MapsPropertiesAndTransactions()
    {
        var transactionDto = new OMTransactionDto
        {
            Id = "txn1",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Processed",
            ReferenceNumber = "ref1234",
            IsImmediate = true,
            IsFulfilledExternally = false,
            ExternalSystem = "Bizagi",
            ExternalSystemId = "CED345678",
            ReceivedDetails = "Received",
            ProcessedDetails = "Processed",
            TransactionType = null,
            Interaction = null,
            Case = null
        };

        var dto = new OMInteractionDto
        {
            Id = "int2",
            Notes = "Interaction notes",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Active",
            ReferenceNumber = "ref9876",
            Case = null,
            Transactions = new List<OMTransactionDto> { transactionDto }
        };

        var entity = DtoToEntityMapper.ToEntity(dto);

        entity.Id.Should().Be(dto.Id);
        entity.Notes.Should().Be(dto.Notes);
        entity.Status.Should().Be(dto.Status);
        entity.ReferenceNumber.Should().Be(dto.ReferenceNumber);
        entity.Transactions.Should().HaveCount(1);
        entity.Transactions.First().Id.Should().Be(transactionDto.Id);
        entity.Transactions.First().ReferenceNumber.Should().Be(transactionDto.ReferenceNumber);
    }

    [Fact]
    public void ToEntity_OMInteractionDto_NullTransactions_InitializesEmptyList()
    {
        var dto = new OMInteractionDto
        {
            Id = "int3",
            Notes = "No transactions",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Inactive",
            ReferenceNumber = "ref1234",
            Case = null,
            Transactions = null
        };

        var entity = DtoToEntityMapper.ToEntity(dto);

        entity.Transactions.Should().NotBeNull();
        entity.Transactions.Should().BeEmpty();
    }

    [Fact]
    public void ToEntity_OMTransactionDto_MapsPropertiesAndNestedEntities()
    {
        var transactionTypeDto = new OMTransactionTypeDto
        {
            Id = "tt1",
            Name = "Type1",
            Description = "Desc",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            RequiresApproval = true
        };

        var dto = new OMTransactionDto
        {
            Id = "txn2",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Status = "Processed",
            IsImmediate = false,
            IsFulfilledExternally = false,
            ExternalSystem = "Bizagi",
            ExternalSystemId = "CED345678",
            ReferenceNumber = "ref1234",
            ReceivedDetails = "Received",
            ProcessedDetails = "Processed",
            TransactionType = transactionTypeDto,
            Interaction = null,
            Case = null
        };

        var entity = DtoToEntityMapper.ToEntity(dto);

        entity.Id.Should().Be(dto.Id);
        entity.Status.Should().Be(dto.Status);
        entity.IsImmediate.Should().Be(dto.IsImmediate);
        entity.IsFulfilledExternally.Should().Be(dto.IsFulfilledExternally);
        entity.ExternalSystem.Should().Be(dto.ExternalSystem);
        entity.ExternalSystemId.Should().Be(dto.ExternalSystemId);
        entity.ReferenceNumber.Should().Be(dto.ReferenceNumber);
        entity.TransactionType.Should().NotBeNull();
        entity.TransactionType.Name.Should().Be(transactionTypeDto.Name);
    }

    [Fact]
    public void ToEntity_OMTransactionTypeDto_MapsProperties()
    {
        var dto = new OMTransactionTypeDto
        {
            Id = "tt2",
            Name = "Type2",
            Description = "Desc2",
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            RequiresApproval = false
        };

        var entity = DtoToEntityMapper.ToEntity(dto);

        entity.Id.Should().Be(dto.Id);
        entity.Name.Should().Be(dto.Name);
        entity.Description.Should().Be(dto.Description);
        entity.RequiresApproval.Should().Be(dto.RequiresApproval);
    }

    [Fact]
    public void ToEntity_OMCaseDto_NullDates_DefaultsToMinValue()
    {
        var dto = new OMCaseDto
        {
            Id = "case3",
            Channel = "Phone",
            CreatedDate = null,
            UpdateDate = null,
            Status = "Pending",
            ReferenceNumber = "ref1234",
            Interactions = null
        };

        var entity = DtoToEntityMapper.ToEntity(dto);

        entity.CreatedDate.Should().Be(DateTime.MinValue);
        entity.UpdateDate.Should().Be(DateTime.MinValue);
    }
}
