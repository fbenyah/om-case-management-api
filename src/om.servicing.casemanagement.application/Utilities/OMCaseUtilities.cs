using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMCaseUtilities
{
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
