using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;
using OM.RequestFramework.Core.Exceptions;
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
    /// Retrieves a list of cases associated with the specified identification number.
    /// </summary>
    /// <remarks>This method queries the case repository to retrieve cases matching the provided
    /// identification number.  If the identification number is invalid, the response will indicate the error without
    /// performing the query. In the event of an exception during the query, the response will include a custom
    /// exception with details about the failure.</remarks>
    /// <param name="identificationNumber">The identification number of the customer whose cases are to be retrieved.  This value cannot be null, empty, or
    /// consist only of whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases associated with the specified identification
    /// number. If the identification number is invalid or an error occurs, the response will include an appropriate
    /// error message or exception.</returns>
    public async Task<OMCaseListResponse> GetCasesForCustomerByIdentificationNumberAsync(string identificationNumber)
    {
        OMCaseListResponse response = new();

        if (string.IsNullOrWhiteSpace(identificationNumber))
        {
            response.SetOrUpdateErrorMessage("Identification number is required.");
            return response;
        }

        try
        {
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.IdentificationNumber == identificationNumber);
            response.Data = OMCaseUtilities.ReturnCaseDtoList(omCases);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving cases for identity number '{identificationNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        return response;
    }

    /// <summary>
    /// Retrieves a list of cases for a customer based on their identification number and case status.
    /// </summary>
    /// <remarks>If either <paramref name="identityNumber"/> or <paramref name="status"/> is null, empty, or
    /// whitespace,  the response will include an error message indicating that both parameters are required. In the
    /// event of an exception during data retrieval, the response will include a custom exception with details about the
    /// error.</remarks>
    /// <param name="identityNumber">The identification number of the customer. Cannot be null, empty, or whitespace.</param>
    /// <param name="status">The status of the cases to retrieve. Cannot be null, empty, or whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases matching the specified identification number
    /// and status. If no cases are found, the <see cref="OMCaseListResponse.Data"/> property will contain an empty
    /// list.</returns>
    public async Task<OMCaseListResponse> GetCasesForCustomerByIdentificationNumberAndStatusAsync(string identityNumber, string status)
    {
        OMCaseListResponse response = new();

        if (string.IsNullOrWhiteSpace(identityNumber)
            || string.IsNullOrWhiteSpace(status))
        {
            response.SetOrUpdateErrorMessage("Both identity number and status are required.");
            return response;
        }

        try
        {
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.Status == status && c.IdentificationNumber == identityNumber);
            response.Data = OMCaseUtilities.ReturnCaseDtoList(omCases);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving cases for identity number '{identityNumber}' with status '{status}'. {ex.Message}";            
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        return response;
    }

    /// <summary>
    /// Retrieves a list of cases associated with the specified reference number.
    /// </summary>
    /// <remarks>This method queries the case repository for cases that match the provided reference number.
    /// If the reference number is invalid, the response will include an error message indicating the issue. In the
    /// event of an exception during the operation, the response will include a custom exception with details about the
    /// error, and the error will be logged.</remarks>
    /// <param name="referenceNumber">The reference number used to identify the cases. This value cannot be null, empty, or consist only of
    /// whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases matching the specified reference number. If no
    /// cases are found, the <see cref="OMCaseListResponse.Data"/> property will be an empty list. If an error occurs,
    /// the response will include an appropriate error message or exception.</returns>
    public async Task<OMCaseListResponse> GetCasesForCustomerByReferenceNumberAsync(string referenceNumber)
    {
        OMCaseListResponse response = new();

        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            response.SetOrUpdateErrorMessage("Reference number is required.");
            return response;
        }

        try
        {
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.ReferenceNumber == referenceNumber);
            response.Data = OMCaseUtilities.ReturnCaseDtoList(omCases);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving cases with reference number '{referenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        return response;
    }

    /// <summary>
    /// Retrieves a list of cases for a customer based on the specified reference number and status.
    /// </summary>
    /// <remarks>This method queries the case repository to find cases that match the provided reference
    /// number and status. If either <paramref name="referenceNumber"/> or <paramref name="status"/> is null, empty, or
    /// whitespace, the method returns a response with an error message indicating that both parameters are required. In
    /// the event of an exception during the query, the method logs the error and returns a response with a custom
    /// exception.</remarks>
    /// <param name="referenceNumber">The unique reference number associated with the customer. This value cannot be null, empty, or whitespace.</param>
    /// <param name="status">The status of the cases to retrieve. This value cannot be null, empty, or whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases that match the specified reference number and
    /// status. If no cases are found, the <see cref="OMCaseListResponse.Data"/> property will contain an empty list. If
    /// an error occurs, the response will include an appropriate error message or exception.</returns>
    public async Task<OMCaseListResponse> GetCasesForCustomerByReferenceNumberAndStatusAsync(string referenceNumber, string status)
    {
        OMCaseListResponse response = new();

        if (string.IsNullOrWhiteSpace(referenceNumber)
            || string.IsNullOrWhiteSpace(status))
        {
            response.SetOrUpdateErrorMessage("Both reference number and status are required.");
            return response;
        }

        try
        {
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.Status == status && c.ReferenceNumber == referenceNumber);
            response.Data = OMCaseUtilities.ReturnCaseDtoList(omCases);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving cases with reference number '{referenceNumber}' with status '{status}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        return response;
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
