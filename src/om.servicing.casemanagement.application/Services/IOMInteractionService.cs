using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMInteractionService
{
    /// <summary>
    /// Retrieves a list of interaction DTOs associated with the specified interaction ID.
    /// </summary>
    /// <param name="id">The unique identifier of the interaction to retrieve. Cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Optional.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects representing the interactions associated with the specified ID.
    /// Returns an empty list if the ID is null, empty, or whitespace, or if no interactions are found.</returns>
    Task<List<OMInteractionDto>> GetInteractionsForInteractionIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of interactions associated with the specified case ID.
    /// </summary>
    /// <param name="caseId">The unique identifier of the case for which interactions are to be retrieved. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
    /// cref="OMInteractionDto"/> objects representing the interactions for the specified case. The list will be empty
    /// if no interactions are found.</returns>
    Task<List<OMInteractionDto>> GetInteractionsForCaseByCaseIdAsync(string caseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of interactions associated with a specific customer identification number.
    /// </summary>
    /// <param name="customerIdentificationNumber">The unique identification number of the customer whose interactions are to be retrieved. This value cannot be
    /// null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
    /// cref="OMInteractionDto"/> objects representing the interactions for the specified customer. If no interactions
    /// are found, the list will be empty.</returns>
    Task<List<OMInteractionDto>> GetInteractionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber, CancellationToken cancellationToken = default);

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
    Task<List<OMInteractionDto>> GetInteractionsForCaseByCaseReferenceNumberAsync(string caseReferenceNumber, CancellationToken cancellationToken = default);

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
    Task<OMInteractionExistsResponse> InteractionExistsWithReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

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
    Task<OMInteractionExistsResponse> InteractionExistsWithIdAsync(string interactionId, CancellationToken cancellationToken = default);

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
    Task<OMInteractionCreateResponse> CreateInteractionAsync(OMInteractionDto omInteractionDto, CancellationToken cancellationToken = default);
}
