using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

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

        return OMCaseUtilities.ReturnCaseDtoList(omCases);
    }

    public async Task<List<OMCaseDto>> GetCasesForCustomerByStatusAsync(string identityNumber, string status)
    {
        if (string.IsNullOrWhiteSpace(identityNumber)
            || string.IsNullOrWhiteSpace(status))
        {
            return new List<OMCaseDto>();
        }

        IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.Status == status && c.IdentificationNumber == identityNumber);

        return OMCaseUtilities.ReturnCaseDtoList(omCases);
    }
}
