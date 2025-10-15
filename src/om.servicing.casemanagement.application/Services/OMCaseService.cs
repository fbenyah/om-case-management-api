using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Mappings;
using om.servicing.casemanagement.domain.Utilities;
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
    public async Task<OMCaseListResponse> GetCasesForCustomerByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        OMCaseListResponse response = new();

        if (string.IsNullOrWhiteSpace(identificationNumber))
        {
            response.SetOrUpdateErrorMessage("Identification number is required.");
            return response;
        }

        try
        {
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.IdentificationNumber == identificationNumber, cancellationToken);
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
    public async Task<OMCaseListResponse> GetCasesForCustomerByIdentificationNumberAndStatusAsync(string identityNumber, string status, CancellationToken cancellationToken = default)
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
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.Status == status && c.IdentificationNumber == identityNumber, cancellationToken);
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
    public async Task<OMCaseListResponse> GetCasesForCustomerByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        OMCaseListResponse response = new();

        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            response.SetOrUpdateErrorMessage("Reference number is required.");
            return response;
        }

        try
        {
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.ReferenceNumber == referenceNumber, cancellationToken);
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
    public async Task<OMCaseListResponse> GetCasesForCustomerByReferenceNumberAndStatusAsync(string referenceNumber, string status, CancellationToken cancellationToken = default)
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
            IEnumerable<OMCase> omCases = await _caseRepository.FindAsync(c => c.Status == status && c.ReferenceNumber == referenceNumber, cancellationToken);
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

    /// <summary>
    /// Creates a new case asynchronously based on the provided case data transfer object (DTO).
    /// </summary>
    /// <remarks>This method generates a unique identifier and reference number for the case, maps the DTO to
    /// an  entity, and persists the entity to the repository. If an error occurs during persistence, the  response will
    /// include a custom exception with details about the failure.</remarks>
    /// <param name="omCaseDto">The data transfer object containing the details of the case to be created.  This parameter cannot be <see
    /// langword="null"/>.</param>
    /// <returns>An <see cref="OMCaseCreateResponse"/> containing the reference number and ID of the created case.  If the input
    /// is <see langword="null"/> or an error occurs during case creation, the response will  indicate the failure and
    /// include relevant error details.</returns>
    public async Task<OMCaseCreateResponse> CreateCaseAsync(OMCaseDto omCaseDto, CancellationToken cancellationToken = default)
    {
        OMCaseCreateResponse response = new();

        if (omCaseDto == null)
        {
            response.SetOrUpdateErrorMessage("Case data is required.");
            return response;
        }
                
        CaseChannel channel = EnumUtils.GetEnumValueFromName<CaseChannel>(omCaseDto.Channel) ?? CaseChannel.Unknown;
        // default to CustomerServicing for now
        OperationalBusinessSegment operationalBusinessSegment = OperationalBusinessSegment.CustomerServicing;
        omCaseDto.Id = UlidUtils.NewUlidString();
        omCaseDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omCaseDto.Id, channel, operationalBusinessSegment);

        await EnsureUniqueCaseIdAndReferenceNumber(omCaseDto, channel, operationalBusinessSegment, cancellationToken);

        OMCase omCase = DtoToEntityMapper.ToEntity(omCaseDto);

        try
        {
            await _caseRepository.AddAsync(omCase, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while attempting to create case '{omCaseDto.IdentificationNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new WritePersistenceException(ex, errorMessage));
            return response;
        }

        response.Data.ReferenceNumber = omCase.ReferenceNumber;
        response.Data.Id = omCase.Id;

        return response;
    }

    /// <summary>
    /// Determines whether a case with the specified ID exists in the repository.
    /// </summary>
    /// <remarks>This method logs an error if an exception occurs while accessing the repository.</remarks>
    /// <param name="caseId">The unique identifier of the case to check. Cannot be null, empty, or whitespace.</param>
    /// <returns><see langword="true"/> if a case with the specified ID exists; otherwise, <see langword="false"/>. Returns <see
    /// langword="false"/> if the <paramref name="caseId"/> is null, empty, or whitespace, or if an error occurs during
    /// the operation.</returns>
    public async Task<bool> CaseExistsWithIdAsync(string caseId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(caseId))
        {
            return false;
        }

        try
        {
            OMCase? omCase = await _caseRepository.GetByIdAsync(caseId, cancellationToken);
            if (omCase != null)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while checking existence of case with ID '{caseId}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);
            return false;
        }
        return false;
    }

    /// <summary>
    /// Asynchronously determines whether a case with the specified reference number exists.
    /// </summary>
    /// <remarks>If the <paramref name="referenceNumber"/> is null, empty, or consists only of whitespace, the
    /// method immediately returns <see langword="false"/>. Logs an error and returns <see langword="false"/> if an
    /// exception occurs during the operation.</remarks>
    /// <param name="referenceNumber">The reference number of the case to check. Cannot be null, empty, or whitespace.</param>
    /// <returns><see langword="true"/> if a case with the specified reference number exists; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> CaseExistsWithReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            return false;
        }

        try
        {
            IEnumerable<OMCase>? omCases = await _caseRepository.FindAsync(c => c.ReferenceNumber == referenceNumber, cancellationToken);
            if (omCases != null && omCases?.Count() > 0)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while checking existence of case with reference number '{referenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);
            return false;
        }
        return false;
    }

    /// <summary>
    /// Ensures that the specified case has a unique identifier and reference number.
    /// </summary>
    /// <remarks>This method checks for existing cases with the same identifier or reference number and
    /// regenerates these values until they are unique. The <paramref name="omCaseDto"/> object is modified in place to
    /// reflect the updated values.</remarks>
    /// <param name="omCaseDto">The case data transfer object containing the case details. The <see cref="OMCaseDto.Id"/> and <see
    /// cref="OMCaseDto.ReferenceNumber"/> properties will be updated to ensure uniqueness.</param>
    /// <param name="channel">The channel associated with the case, used to generate a unique reference number.</param>
    /// <param name="operationalBusinessSegment">The operational business segment associated with the case, used to generate a unique reference number.</param>
    /// <returns></returns>
    private async Task EnsureUniqueCaseIdAndReferenceNumber(OMCaseDto omCaseDto, CaseChannel channel, OperationalBusinessSegment operationalBusinessSegment, CancellationToken cancellationToken = default)
    {
        // ensure that there is no existing case with the same id
        // if there is, generate a new id and keep checking till it is unique
        while (await CaseExistsWithIdAsync(omCaseDto.Id, cancellationToken))
        {
            omCaseDto.Id = UlidUtils.NewUlidString();
            omCaseDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omCaseDto.Id, channel, operationalBusinessSegment);
        }

        // ensure that there is no existing case with the same reference number
        // if there is, generate a new reference number and keep checking till it is unique
        while (await CaseExistsWithReferenceNumberAsync(omCaseDto.ReferenceNumber, cancellationToken))
        {
            omCaseDto.Id = UlidUtils.NewUlidString();
            omCaseDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omCaseDto.Id, channel, operationalBusinessSegment);
        }
    }
}
