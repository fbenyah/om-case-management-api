using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Application.Utilities;

public class OMTransactionUtilitiesTests
{
    [Fact]
    public void ReturnTransactionDtoList_NullInput_ReturnsEmptyList()
    {
        var result = OMTransactionUtilities.ReturnTransactionDtoList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionDtoList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMTransactionUtilities.ReturnTransactionDtoList(Enumerable.Empty<OMTransaction>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionDtoList_ValidInput_ReturnsMappedDtos()
    {
        var transactions = new List<OMTransaction>
        {
            new OMTransaction { CaseId = "C1", InteractionId = "I1", TransactionTypeId = "T1", IsImmediate = true, IsFulfilledExternally = true, ExternalSystem = "Bizagi", ExternalSystemId = "CED12345", ReceivedDetails = "R1", ProcessedDetails = "P1", Status = "Active" }
        };

        // Arrange: Mock the mapper if possible, otherwise rely on actual mapping
        var result = OMTransactionUtilities.ReturnTransactionDtoList(transactions);

        Assert.Single(result);
        Assert.Equal("R1", result[0].ReceivedDetails);
        Assert.Equal("P1", result[0].ProcessedDetails);
        Assert.Equal(true, result[0].IsImmediate);
        Assert.Equal(true, result[0].IsFulfilledExternally);
        Assert.Equal("Bizagi", result[0].ExternalSystem);
        Assert.Equal("CED12345", result[0].ExternalSystemId);
        Assert.Equal("Active", result[0].Status);
    }

    [Fact]
    public void ReturnTransactionList_NullInput_ReturnsEmptyList()
    {
        var result = OMTransactionUtilities.ReturnTransactionList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMTransactionUtilities.ReturnTransactionList(new List<OMTransactionDto>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionList_ValidInput_ReturnsMappedEntities()
    {
        var dtos = new List<OMTransactionDto>
        {
            new OMTransactionDto { ReceivedDetails = "R2", ProcessedDetails = "P2", IsImmediate = false, IsFulfilledExternally = false, Status = "Inactive" }
        };

        var result = OMTransactionUtilities.ReturnTransactionList(dtos);

        Assert.Single(result);
        Assert.Equal("R2", result[0].ReceivedDetails);
        Assert.Equal("P2", result[0].ProcessedDetails);
        Assert.False(result[0].IsImmediate);
        Assert.False(result[0].IsFulfilledExternally);
        Assert.Equal("Inactive", result[0].Status);
    }
}
