using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMCaseUtilities
{
    /// <summary>
    /// Converts a collection of <see cref="OMCase"/> objects to a list of <see cref="OMCaseDto"/> objects.
    /// </summary>
    /// <param name="omCases">The collection of <see cref="OMCase"/> objects to convert. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMCaseDto"/> objects representing the converted cases.  Returns an empty list if <paramref
    /// name="omCases"/> is null or contains no elements.</returns>
    public static List<OMCaseDto> ReturnCaseDtoList(IEnumerable<OMCase> omCases)
    {
        List<OMCaseDto> caseDtoList = new List<OMCaseDto>();

        if (omCases == null || !omCases.Any())
        {
            return caseDtoList;
        }

        foreach (OMCase omCase in omCases)
        {
            caseDtoList.Add(EntityToDtoMapper.ToDto(omCase));
        }

        return caseDtoList;
    }

    /// <summary>
    /// Converts a list of data transfer objects (DTOs) to a list of entity objects.
    /// </summary>
    /// <remarks>This method iterates through the provided list of DTOs and maps each item to its
    /// corresponding entity object  using the <see cref="DtoToEntityMapper.ToEntity"/> method.</remarks>
    /// <param name="caseDtoList">The list of <see cref="OMCaseDto"/> objects to be converted. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMCase"/> objects converted from the provided DTOs.  Returns an empty list if <paramref
    /// name="caseDtoList"/> is null or empty.</returns>
    public static List<OMCase> ReturnCaseList(List<OMCaseDto> caseDtoList)
    {
        List<OMCase> caseList = new List<OMCase>();

        if (caseDtoList == null || !caseDtoList.Any())
        {
            return caseList;
        }

        foreach (OMCaseDto omCaseDto in caseDtoList)
        {
            caseList.Add(DtoToEntityMapper.ToEntity(omCaseDto));
        }

        return caseList;
    }
}
