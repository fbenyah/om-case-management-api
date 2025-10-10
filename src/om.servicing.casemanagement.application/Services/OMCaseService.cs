using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.application.Services;

public class OMCaseService : IOMCaseService
{
    private readonly IGenericRepository<OMCase> _caseRepository;

    public OMCaseService(IGenericRepository<OMCase> caseRepository)
    {
        _caseRepository = caseRepository;
    }

    public async Task<List<OMCaseDto>> GetCasesForCustomer(string identityNumber)
    {
        if (string.IsNullOrWhiteSpace(identityNumber))
        {
            return new List<OMCaseDto>();
        }

        IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.IdentificationNumber == identityNumber);

        return ReturnCaseListDto(omCases);
    }

    public async Task<List<OMCaseDto>> GetCasesForCustomerByStatusAsync(string identityNumber, string status)
    {
        if (string.IsNullOrWhiteSpace(identityNumber)
            || string.IsNullOrWhiteSpace(status))
        {
            return new List<OMCaseDto>();
        }

        IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.Status == status && c.IdentificationNumber == identityNumber);

        return ReturnCaseListDto(omCases);
    }

    private List<OMCaseDto> ReturnCaseListDto(IEnumerable<OMCase> omCases)
    {
        List<OMCaseDto> caseListDto = new List<OMCaseDto>();

        if (omCases == null || !omCases.Any())
        {
            return caseListDto;
        }

        foreach (OMCase omCase in omCases)
        {
            caseListDto.Add(EntityToDtoMapper.ToDto(omCase));
        }

        return caseListDto;
    }
}
