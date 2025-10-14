using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Application.Utilities;

public class OMInteractionUtilitiesTests
{
    [Fact]
    public void ReturnInteractionDtoList_NullInput_ReturnsEmptyList()
    {
        var result = OMInteractionUtilities.ReturnInteractionDtoList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnInteractionDtoList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMInteractionUtilities.ReturnInteractionDtoList(Enumerable.Empty<OMInteraction>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnInteractionDtoList_ValidInput_ReturnsMappedDtos()
    {
        var interactions = new List<OMInteraction>
    {
        new OMInteraction { CaseId = "C1", Notes = "Note1", Status = "Active" }
    };

        var result = OMInteractionUtilities.ReturnInteractionDtoList(interactions);

        Assert.Single(result);
        Assert.Equal("Note1", result[0].Notes);
        Assert.Equal("Active", result[0].Status);
    }

    [Fact]
    public void ReturnInteractionList_NullInput_ReturnsEmptyList()
    {
        var result = OMInteractionUtilities.ReturnInteractionList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnInteractionList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMInteractionUtilities.ReturnInteractionList(new List<OMInteractionDto>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnInteractionList_ValidInput_ReturnsMappedEntities()
    {
        var dtos = new List<OMInteractionDto>
    {
        new OMInteractionDto { Notes = "Note2", Status = "Inactive" }
    };

        var result = OMInteractionUtilities.ReturnInteractionList(dtos);

        Assert.Single(result);
        Assert.Equal("Note2", result[0].Notes);
        Assert.Equal("Inactive", result[0].Status);
    }
}
