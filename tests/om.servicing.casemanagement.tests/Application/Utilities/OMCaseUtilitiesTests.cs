using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.tests.Application.Utilities;

public class OMCaseUtilitiesTests
{
    [Fact]
    public void ReturnCaseDtoList_NullInput_ReturnsEmptyList()
    {
        var result = OMCaseUtilities.ReturnCaseDtoList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnCaseDtoList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMCaseUtilities.ReturnCaseDtoList(new List<OMCase>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnCaseDtoList_ValidInput_ReturnsMappedList()
    {
        var cases = new List<OMCase>
        {
            new OMCase { Channel = "Email", Status = "Open", IdentificationNumber = "123" },
            new OMCase { Channel = "Phone", Status = "Closed", IdentificationNumber = "456" }
        };

        // Optionally, mock EntityToDtoMapper.ToDto if needed
        var result = OMCaseUtilities.ReturnCaseDtoList(cases);

        Assert.Equal(2, result.Count);
        Assert.All(result, dto => Assert.IsType<OMCaseDto>(dto));
    }

    [Fact]
    public void ReturnCaseList_NullInput_ReturnsEmptyList()
    {
        var result = OMCaseUtilities.ReturnCaseList(null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnCaseList_EmptyInput_ReturnsEmptyList()
    {
        var result = OMCaseUtilities.ReturnCaseList(new List<OMCaseDto>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReturnCaseList_ValidInput_ReturnsMappedList()
    {
        var dtos = new List<OMCaseDto>
        {
            new OMCaseDto { Channel = "Email", Status = "Open" },
            new OMCaseDto { Channel = "Phone", Status = "Closed" }
        };

        // Optionally, mock DtoToEntityMapper.ToEntity if needed
        var result = OMCaseUtilities.ReturnCaseList(dtos);

        Assert.Equal(2, result.Count);
        Assert.All(result, entity => Assert.IsType<OMCase>(entity));
    }
}
