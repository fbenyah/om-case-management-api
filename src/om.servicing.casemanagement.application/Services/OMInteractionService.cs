using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Services;

public class OMInteractionService : BaseService, IOMInteractionService
{
    private readonly IOMCaseService _omCaseService;
    private readonly IGenericRepository<OMInteraction> _interactionRepository;

    public OMInteractionService(
        IOMCaseService oMCaseService,
        ILoggingService loggingService,
        IGenericRepository<OMInteraction> interactionRepository
        ) : base(loggingService)
    {
        _omCaseService = oMCaseService;
        _interactionRepository = interactionRepository;
    }

    /// <summary>
    /// Retrieves a list of interaction DTOs associated with the specified interaction ID.
    /// </summary>
    /// <param name="id">The unique identifier of the interaction to retrieve. Cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Optional.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects representing the interactions associated with the specified ID.
    /// Returns an empty list if the ID is null, empty, or whitespace, or if no interactions are found.</returns>
    public async Task<List<OMInteractionDto>> GetInteractionsForInteractionIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return new List<OMInteractionDto>();
        }

        IEnumerable<OMInteraction> omInteractions = await _interactionRepository.FindAsync(t => t.Id == id);

        return OMInteractionUtilities.ReturnInteractionDtoList(omInteractions);
    }

    /// <summary>
    /// Retrieves a list of interactions associated with the specified case ID.
    /// </summary>
    /// <remarks>This method queries the interaction repository to find interactions associated with the given
    /// case ID and converts them into a list of data transfer objects (<see cref="OMInteractionDto"/>).</remarks>
    /// <param name="caseId">The unique identifier of the case for which interactions are to be retrieved. Cannot be null, empty, or
    /// whitespace.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects representing the interactions for the specified case. Returns
    /// an empty list if the <paramref name="caseId"/> is null, empty, or whitespace, or if no interactions are found.</returns>
    public async Task<List<OMInteractionDto>> GetInteractionsForCaseByCaseIdAsync(string caseId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(caseId))
        {
            return new List<OMInteractionDto>();
        }

        IEnumerable<OMInteraction> omInteractions = await _interactionRepository.FindAsync(t => t.CaseId == caseId);

        return OMInteractionUtilities.ReturnInteractionDtoList(omInteractions);
    }

    /// <summary>
    /// Retrieves a list of interactions associated with the cases for a given customer identification number.
    /// </summary>
    /// <remarks>This method aggregates interactions from all cases associated with the specified customer. If
    /// no cases are found for the customer, or if the customer identification number is invalid, the method returns an
    /// empty list.</remarks>
    /// <param name="customerIdentificationNumber">The unique identification number of the customer. This value cannot be null, empty, or consist only of
    /// whitespace.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects representing the interactions associated with the customer's
    /// cases. Returns an empty list if the customer has no cases or if the provided identification number is invalid.</returns>
    public async Task<List<OMInteractionDto>> GetInteractionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerIdentificationNumber))
        {
            return new List<OMInteractionDto>();
        }

        OMCaseListResponse omCaseListResponse = await _omCaseService.GetCasesForCustomerByIdentificationNumberAsync(customerIdentificationNumber);

        if (!omCaseListResponse.Success)
        {
            return new List<OMInteractionDto>();
        }

        if (omCaseListResponse.Data == null || !omCaseListResponse.Data.Any())
        {
            return new List<OMInteractionDto>();
        }

        List<OMInteractionDto> allInteractionsDto = new();

        foreach (OMCaseDto omCaseDto in omCaseListResponse.Data)
        {
            if (omCaseDto.Interactions != null && omCaseDto.Interactions.Any())
            {
                allInteractionsDto.AddRange(omCaseDto.Interactions);
                continue;
            }

            // if Interactions not populated in case, fetch from repo
            List<OMInteractionDto> interactionsForCase = await GetInteractionsForCaseByCaseIdAsync(omCaseDto.Id);
            if (interactionsForCase != null && interactionsForCase.Any())
            {
                allInteractionsDto.AddRange(interactionsForCase);
            }
        }

        return allInteractionsDto;
    }

    /// <summary>
    /// Retrieves a list of interactions associated with a case identified by the specified case reference number.
    /// </summary>
    /// <remarks>This method first retrieves cases associated with the given case reference number. If
    /// interactions are not directly available in the retrieved cases, it attempts to fetch them from a repository
    /// using the case IDs. The method ensures that all interactions related to the specified case are aggregated and
    /// returned.</remarks>
    /// <param name="caseReferenceNumber">The unique reference number of the case for which interactions are to be retrieved. Cannot be null, empty, or
    /// whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects representing the interactions associated with the specified
    /// case. Returns an empty list if the case reference number is invalid, no cases are found, or no interactions are
    /// associated with the case.</returns>
    public async Task<List<OMInteractionDto>> GetInteractionsForCaseByCaseReferenceNumberAsync(string caseReferenceNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(caseReferenceNumber))
        {
            return new List<OMInteractionDto>();
        }

        OMCaseListResponse omCaseListResponse = await _omCaseService.GetCasesForCustomerByReferenceNumberAsync(caseReferenceNumber);

        if (!omCaseListResponse.Success)
        {
            return new List<OMInteractionDto>();
        }

        if (omCaseListResponse.Data == null || !omCaseListResponse.Data.Any())
        {
            return new List<OMInteractionDto>();
        }

        List<OMInteractionDto> allInteractionsDto = new();

        foreach (OMCaseDto omCaseDto in omCaseListResponse.Data)
        {
            if (omCaseDto.Interactions != null && omCaseDto.Interactions.Any())
            {
                allInteractionsDto.AddRange(omCaseDto.Interactions);
                continue;
            }

            // if Interactions not populated in case, fetch from repo
            List<OMInteractionDto> interactionsForCase = await GetInteractionsForCaseByCaseIdAsync(omCaseDto.Id);
            if (interactionsForCase != null && interactionsForCase.Any())
            {
                allInteractionsDto.AddRange(interactionsForCase);
            }
        }

        return allInteractionsDto;
    }

    /// <summary>
    /// Asynchronously determines whether an interaction exists with the specified reference number.
    /// </summary>
    /// <remarks>If an error occurs during the operation, the response will include details about the
    /// exception encountered.</remarks>
    /// <param name="referenceNumber">The reference number to search for. This value cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="OMInteractionExistsResponse"/> object  indicating whether an interaction with the specified reference
    /// number exists. If the reference number is invalid, the response  will include an error message.</returns>
    public async Task<OMInteractionExistsResponse> InteractionExistsWithReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        OMInteractionExistsResponse response = new();

        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            response.SetOrUpdateErrorMessage("Reference number is required.");
            return response;
        }

        try
        {
            IEnumerable<OMInteraction>? omInteractions= await _interactionRepository.FindAsync(c => c.ReferenceNumber == referenceNumber, cancellationToken);
            if (omInteractions != null && omInteractions?.Count() > 0)
            {
                response.Data = true;
                return response;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while checking existence of interaction with reference number '{referenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
        }
        return response;
    }

    /// <summary>
    /// Determines whether an interaction with the specified identifier exists in the repository.
    /// </summary>
    /// <remarks>If the <paramref name="interactionId"/> is null, empty, or consists only of whitespace, the
    /// response will include an error message. If an error occurs during the repository query, the response will
    /// include a custom exception with details about the failure.</remarks>
    /// <param name="interactionId">The unique identifier of the interaction to check. Cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>An <see cref="OMInteractionExistsResponse"/> object containing a boolean value indicating whether the
    /// interaction exists  and any associated error messages or exceptions.</returns>
    public async Task<OMInteractionExistsResponse> InteractionExistsWithIdAsync(string interactionId, CancellationToken cancellationToken = default)
    {
        OMInteractionExistsResponse response = new();

        if (string.IsNullOrWhiteSpace(interactionId))
        {
            response.SetOrUpdateErrorMessage("Interaction Id is required.");
            return response;
        }

        try
        {
            IEnumerable<OMInteraction>? omInteractions = await _interactionRepository.FindAsync(c => c.Id == interactionId, cancellationToken);
            if (omInteractions != null && omInteractions?.Count() > 0)
            {
                response.Data = true;
                return response;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while checking existence of interaction with reference number '{interactionId}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
        }
        return response;
    }
}
