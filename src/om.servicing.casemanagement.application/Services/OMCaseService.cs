using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Services;

public class OMCaseService : BaseService, IOMCaseService
{
    private readonly IGenericRepository<OMCase> _caseRepository;

    public OMCaseService(
        ILoggingService loggingService,
        IGenericRepository<OMCase> caseRepository        
        ) : base(loggingService)
    {
        _caseRepository = caseRepository;
    }

    /// <summary>
    /// Retrieves a list of cases associated with the specified customer identity number.
    /// </summary>
    /// <param name="identityNumber">The identity number of the customer whose cases are to be retrieved.  Cannot be null, empty, or consist only of
    /// whitespace.</param>
    /// <returns>A list of <see cref="OMCaseDto"/> objects representing the cases associated with the specified identity number. 
    /// Returns an empty list if the identity number is null, empty, or no cases are found.</returns>
    public async Task<List<OMCaseDto>> GetCasesForCustomer(string identityNumber)
    {
        if (string.IsNullOrWhiteSpace(identityNumber))
        {
            return new List<OMCaseDto>();
        }

        IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.IdentificationNumber == identityNumber);

        return OMCaseUtilities.ReturnCaseDtoList(omCases);
    }

    /// <summary>
    /// Retrieves a list of cases for a specific customer based on their identity number and case status.
    /// </summary>
    /// <remarks>This method queries the case repository to find cases that match the specified criteria and
    /// converts them into DTOs for external use. Ensure that both <paramref name="identityNumber"/> and <paramref
    /// name="status"/> are valid non-empty strings before calling this method.</remarks>
    /// <param name="identityNumber">The identity number of the customer. This value cannot be null, empty, or consist only of whitespace.</param>
    /// <param name="status">The status of the cases to retrieve. This value cannot be null, empty, or consist only of whitespace.</param>
    /// <returns>A list of <see cref="OMCaseDto"/> objects representing the cases that match the specified identity number and
    /// status. Returns an empty list if no matching cases are found or if the input parameters are invalid.</returns>
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

    // create a case
    public async Task<bool> CreateCaseAsync(OMCaseDto omCaseDto)
    {
        if (omCaseDto == null)
        {
            return false;
        }
        OMCase omCase = DtoToEntityMapper.ToEntity(omCaseDto);

        await _caseRepository.AddAsync(omCase);
        return true;
    }
}
