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
    /// <returns>An <see cref="OMInteractionListResponse"/> objects representing the interactions associated with the specified ID.
    /// Returns an empty list if the ID is null, empty, or whitespace, or if no interactions are found.</returns>
    public async Task<OMInteractionListResponse> GetInteractionsForInteractionIdAsync(string id, CancellationToken cancellationToken = default)
    {
        OMInteractionListResponse response = new();

        if (string.IsNullOrWhiteSpace(id))
        {
            response.SetOrUpdateErrorMessage("Interaction Id is required.");
            return response;
        }

        try
        {
            IEnumerable<OMInteraction> omInteractions = await _interactionRepository.FindAsync(t => t.Id == id);

            if (omInteractions == null || !omInteractions.Any())
            {
                response.SetOrUpdateCustomException(new NotFoundException($"No interactions found for id '{id}'."));
                return response;
            }

            response.Data = OMInteractionUtilities.ReturnInteractionDtoList(omInteractions);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving interactions with id '{id}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }
        
        return response;
    }

    /// <summary>
    /// Retrieves a list of interactions associated with the specified case ID.
    /// </summary>
    /// <remarks>This method queries the interaction repository to find interactions associated with the given
    /// case ID and converts them into a list of data transfer objects (<see cref="OMInteractionDto"/>).</remarks>
    /// <param name="caseId">The unique identifier of the case for which interactions are to be retrieved. Cannot be null, empty, or
    /// whitespace.</param>
    /// <returns>An <see cref="OMInteractionListResponse"/> objects representing the interactions for the specified case. Returns
    /// an empty list if the <paramref name="caseId"/> is null, empty, or whitespace, or if no interactions are found.</returns>
    public async Task<OMInteractionListResponse> GetInteractionsForCaseByCaseIdAsync(string caseId, CancellationToken cancellationToken = default)
    {
        OMInteractionListResponse response = new();

        if (string.IsNullOrWhiteSpace(caseId))
        {
            response.SetOrUpdateErrorMessage("Case Id is required.");
            return response;
        }        

        try
        {
            IEnumerable<OMInteraction> omInteractions = await _interactionRepository.FindAsync(t => t.CaseId == caseId);

            if (omInteractions == null || !omInteractions.Any())
            {
                response.SetOrUpdateCustomException(new NotFoundException($"No interactions found for case id '{caseId}'."));
                return response;
            }

            response.Data = OMInteractionUtilities.ReturnInteractionDtoList(omInteractions);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving interactions with case Id '{caseId}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }
        
        return response;
    }

    /// <summary>
    /// Retrieves a list of interactions associated with the cases for a given customer identification number.
    /// </summary>
    /// <remarks>This method aggregates interactions from all cases associated with the specified customer. If
    /// no cases are found for the customer, or if the customer identification number is invalid, the method returns an
    /// empty list.</remarks>
    /// <param name="customerIdentificationNumber">The unique identification number of the customer. This value cannot be null, empty, or consist only of
    /// whitespace.</param>
    /// <returns>An <see cref="OMInteractionListResponse"/> objects representing the interactions associated with the customer's
    /// cases. Returns an empty list if the customer has no cases or if the provided identification number is invalid.</returns>
    public async Task<OMInteractionListResponse> GetInteractionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber, CancellationToken cancellationToken = default)
    {
        OMInteractionListResponse response = new();

        if (string.IsNullOrWhiteSpace(customerIdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Customer identification number is required.");
            return response;
        }

        OMCaseListResponse omCaseListResponse;

        try
        {
            omCaseListResponse = await _omCaseService.GetCasesForCustomerByIdentificationNumberAsync(customerIdentificationNumber);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving cases for customer by identification number to get interactions '{customerIdentificationNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        if (!omCaseListResponse.Success)
        {
            response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);

            if (omCaseListResponse.CustomExceptions != null)
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }

            return response;
        }

        if (omCaseListResponse.Data == null || !omCaseListResponse.Data.Any())
        {
            response.SetOrUpdateCustomException(new NotFoundException($"No cases found for customer identification number '{customerIdentificationNumber}'."));
            return response;
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
            OMInteractionListResponse interactionsForCaseResponse;

            try
            {
                interactionsForCaseResponse = await GetInteractionsForCaseByCaseIdAsync(omCaseDto.Id);
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred while retrieving interactions for case by case id '{omCaseDto.Id}'. {ex.Message}";
                _loggingService.LogError(errorMessage, ex);

                response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
                return response;
            }

            if (!interactionsForCaseResponse.Success)
            {
                response.SetOrUpdateErrorMessages(interactionsForCaseResponse.ErrorMessages);

                if (interactionsForCaseResponse.CustomExceptions != null)
                {
                    response.SetOrUpdateCustomExceptions(interactionsForCaseResponse.CustomExceptions);
                }

                return response;
            }

            if (interactionsForCaseResponse.Data == null || !interactionsForCaseResponse.Data.Any())
            {
                response.SetOrUpdateCustomException(new NotFoundException($"No interactions found for case with id '{omCaseDto.Id}'."));
                return response;
            }

            allInteractionsDto.AddRange(interactionsForCaseResponse.Data);
        }

        response.Data = allInteractionsDto;
        return response;
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
    /// <returns>An <see cref="OMInteractionListResponse"/> objects representing the interactions associated with the specified
    /// case. Returns an empty list if the case reference number is invalid, no cases are found, or no interactions are
    /// associated with the case.</returns>
    public async Task<OMInteractionListResponse> GetInteractionsForCaseByCaseReferenceNumberAsync(string caseReferenceNumber, CancellationToken cancellationToken = default)
    {
        OMInteractionListResponse response = new();

        if (string.IsNullOrWhiteSpace(caseReferenceNumber))
        {
            response.SetOrUpdateErrorMessage("Case reference number is required.");
            return response;
        }

        OMCaseListResponse omCaseListResponse;
        try
        {
            omCaseListResponse = await _omCaseService.GetCasesForCustomerByReferenceNumberAsync(caseReferenceNumber);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving cases for customer by case reference number '{caseReferenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        if (!omCaseListResponse.Success)
        {
            response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);

            if (omCaseListResponse.CustomExceptions != null)
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }

            return response;
        }

        if (omCaseListResponse.Data == null || !omCaseListResponse.Data.Any())
        {
            response.SetOrUpdateCustomException(new NotFoundException($"No cases found for case reference number '{caseReferenceNumber}'."));
            return response;
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
            OMInteractionListResponse interactionsForCaseResponse;

            try
            {
                interactionsForCaseResponse = await GetInteractionsForCaseByCaseIdAsync(omCaseDto.Id);
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred while retrieving interactions for case by case id '{omCaseDto.Id}'. {ex.Message}";
                _loggingService.LogError(errorMessage, ex);

                response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
                return response;
            }

            if (!interactionsForCaseResponse.Success)
            {
                response.SetOrUpdateErrorMessages(interactionsForCaseResponse.ErrorMessages);

                if (interactionsForCaseResponse.CustomExceptions != null)
                {
                    response.SetOrUpdateCustomExceptions(interactionsForCaseResponse.CustomExceptions);
                }

                return response;
            }

            if (interactionsForCaseResponse.Data == null || !interactionsForCaseResponse.Data.Any())
            {
                response.SetOrUpdateCustomException(new NotFoundException($"No interactions found for case with id '{omCaseDto.Id}'."));
                return response;
            }

            allInteractionsDto.AddRange(interactionsForCaseResponse.Data);
        }

        response.Data = allInteractionsDto;
        return response;
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
    
    /// <summary>
    /// Creates a new interaction asynchronously based on the provided interaction data.
    /// </summary>
    /// <remarks>This method validates the provided interaction data, ensures unique identifiers and reference
    /// numbers,  and persists the interaction to the repository. If an error occurs during persistence, the response 
    /// will include a custom exception with detailed error information.</remarks>
    /// <param name="omInteractionDto">The data transfer object containing the details of the interaction to be created.  This parameter must not be
    /// <see langword="null"/> and must include valid case data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A <see cref="OMInteractionCreateResponse"/> containing the result of the operation, including the  newly created
    /// interaction's ID, case ID, and reference numbers. If the operation fails, the response  will include error
    /// details.</returns>
    public async Task<OMInteractionCreateResponse> CreateInteractionAsync(OMInteractionDto omInteractionDto, CancellationToken cancellationToken = default)
    {
        OMInteractionCreateResponse response = new();

        if (omInteractionDto == null)
        {
            response.SetOrUpdateErrorMessage("Interaction data is required.");
            return response;
        }

        if (omInteractionDto.Case == null)
        {
            response.SetOrUpdateErrorMessage("Case data for interaction is required.");
            return response;
        }

        CaseChannel channel = EnumUtils.GetEnumValueFromName<CaseChannel>(omInteractionDto.Case!.Channel) ?? CaseChannel.Unknown;
        // default to CustomerServicing for now
        OperationalBusinessSegment operationalBusinessSegment = OperationalBusinessSegment.CustomerServicing;
        omInteractionDto.CreatedDate = DateTime.Now;
        omInteractionDto.Id = UlidUtils.NewUlidString();
        omInteractionDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omInteractionDto.Id, channel, operationalBusinessSegment);

        await EnsureUniqueInteractionIdAndReferenceNumber(omInteractionDto, channel, operationalBusinessSegment, cancellationToken);

        OMInteraction omInteraction = DtoToEntityMapper.ToEntity(omInteractionDto);

        try
        {
            await _interactionRepository.AddAsync(omInteraction, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while attempting to create interaction for case '{omInteraction.Case.Id}' with reference '{omInteraction.Case.ReferenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new WritePersistenceException(ex, errorMessage));
            return response;
        }
        
        response.Data.Id = omInteraction.Id;
        response.Data.CaseId = omInteraction.Case.Id;
        response.Data.ReferenceNumber = omInteraction.ReferenceNumber;
        response.Data.CaseReferenceNumber = omInteraction.Case.ReferenceNumber;

        return response;
    }

    /// <summary>
    /// Ensures that the specified interaction has a unique interaction ID and reference number.
    /// </summary>
    /// <remarks>This method validates the uniqueness of both the interaction ID and reference number. If
    /// either is found to be non-unique, it regenerates the values and retries the validation process up to a maximum
    /// number of attempts. If uniqueness cannot be ensured after the maximum attempts, an <see
    /// cref="InvalidOperationException"/> is thrown.</remarks>
    /// <param name="omInteractionDto">The interaction data transfer object containing the ID and reference number to validate and potentially
    /// regenerate.</param>
    /// <param name="channel">The channel associated with the interaction, used to generate a new reference number if needed.</param>
    /// <param name="operationalBusinessSegment">The operational business segment associated with the interaction, used to generate a new reference number if
    /// needed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the uniqueness of the interaction ID or reference number cannot be verified due to an unexpected
    /// state, or if a unique value cannot be generated after the maximum number of attempts.</exception>
    private async Task EnsureUniqueInteractionIdAndReferenceNumber(OMInteractionDto omInteractionDto, CaseChannel channel, OperationalBusinessSegment operationalBusinessSegment, CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 10;
        int attempts = 0;

        // ensure that there is no existing interaction with the same id
        while (true)
        {
            var idExistsResponse = await InteractionExistsWithIdAsync(omInteractionDto.Id, cancellationToken);
            if (!idExistsResponse.Success)
            {
                // If we cannot determine existence, stop and surface the issue.
                // We are choosing the throw here because this is an unexpected state that we cannot recover from.
                throw new InvalidOperationException($"Unable to verify interaction id uniqueness: {string.Join("; ", idExistsResponse.ErrorMessages ?? new List<string>())}");
            }

            if (!idExistsResponse.Data)
            {
                break; // id is unique
            }

            // regenerate and retry
            omInteractionDto.Id = UlidUtils.NewUlidString();
            omInteractionDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omInteractionDto.Id, channel, operationalBusinessSegment);

            if (++attempts >= maxAttempts)
            {
                throw new InvalidOperationException($"Unable to generate a unique interaction id after {attempts} attempts.");
            }
        }

        // ensure that there is no existing interaction with the same reference number
        attempts = 0;
        while (true)
        {
            var refExistsResponse = await InteractionExistsWithReferenceNumberAsync(omInteractionDto.ReferenceNumber, cancellationToken);
            if (!refExistsResponse.Success)
            {
                throw new InvalidOperationException($"Unable to verify reference number uniqueness for interaction: {string.Join("; ", refExistsResponse.ErrorMessages ?? new List<string>())}");
            }

            if (!refExistsResponse.Data)
            {
                break; // reference number is unique
            }

            // regenerate and retry
            omInteractionDto.Id = UlidUtils.NewUlidString();
            omInteractionDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omInteractionDto.Id, channel, operationalBusinessSegment);

            if (++attempts >= maxAttempts)
            {
                throw new InvalidOperationException($"Unable to generate a unique interaction reference number after {attempts} attempts.");
            }
        }
    }
}
