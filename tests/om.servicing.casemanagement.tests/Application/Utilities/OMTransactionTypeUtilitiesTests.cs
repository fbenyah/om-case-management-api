using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Application.Utilities;

public class OMTransactionTypeUtilitiesTests
{
    [Fact]
    public void ReturnTransactionTypeDtoList_NullInput_ReturnsEmptyList()
    {
        var result = OMTransactionTypeUtilities.ReturnTransactionTypeDtoList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionTypeDtoList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMTransactionTypeUtilities.ReturnTransactionTypeDtoList(Enumerable.Empty<OMTransactionType>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionTypeDtoList_ValidInput_ReturnsMappedDtos()
    {
        var transactionTypes = new List<OMTransactionType>
    {
        new OMTransactionType
        {
            Id = "type1",
            Name = "Payment",
            Description = "Handles payment transactions",
            RequiresApproval = true,
            CreatedDate = new DateTime(2024, 1, 1),
            UpdateDate = new DateTime(2024, 2, 1)
        }
    };

        var result = OMTransactionTypeUtilities.ReturnTransactionTypeDtoList(transactionTypes);

        Assert.Single(result);
        Assert.Equal("type1", result[0].Id);
        Assert.Equal("Payment", result[0].Name);
        Assert.Equal("Handles payment transactions", result[0].Description);
        Assert.True(result[0].RequiresApproval);
        Assert.Equal(new DateTime(2024, 1, 1), result[0].CreatedDate);
        Assert.Equal(new DateTime(2024, 2, 1), result[0].UpdateDate);
    }

    [Fact]
    public void ReturnTransactionTypeList_NullInput_ReturnsEmptyList()
    {
        var result = OMTransactionTypeUtilities.ReturnTransactionTypeList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionTypeList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMTransactionTypeUtilities.ReturnTransactionTypeList(new List<OMTransactionTypeDto>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnTransactionTypeList_ValidInput_ReturnsMappedEntities()
    {
        var dtos = new List<OMTransactionTypeDto>
    {
        new OMTransactionTypeDto
        {
            Id = "type2",
            Name = "Refund",
            Description = "Handles refund transactions",
            RequiresApproval = false,
            CreatedDate = new DateTime(2024, 3, 1),
            UpdateDate = new DateTime(2024, 4, 1)
        }
    };

        var result = OMTransactionTypeUtilities.ReturnTransactionTypeList(dtos);

        Assert.Single(result);
        Assert.Equal("type2", result[0].Id);
        Assert.Equal("Refund", result[0].Name);
        Assert.Equal("Handles refund transactions", result[0].Description);
        Assert.False(result[0].RequiresApproval);
        Assert.Equal(new DateTime(2024, 3, 1), result[0].CreatedDate);
        Assert.Equal(new DateTime(2024, 4, 1), result[0].UpdateDate);
    }
}
