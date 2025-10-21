using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMTransactionService
{
    /// <summary>
    /// Retrieves a list of transactions associated with the specified case ID.
    /// </summary>
    /// <param name="caseId">The unique identifier of the case for which transactions are to be retrieved. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
    /// cref="OMTransactionDto"/> objects  representing the transactions for the specified case. Returns an empty list
    /// if no transactions are found.</returns>
    Task<OMTransactionListResponse> GetTransactionsForCaseByCaseIdAsync(string caseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific case, filtered by the customer's identification
    /// number.
    /// </summary>
    /// <param name="customerIdentificationNumber">The identification number of the customer whose transactions are to be retrieved.  This value cannot be null or
    /// empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
    /// cref="OMTransactionDto"/> objects representing the transactions for the specified customer. If no transactions
    /// are found, the list will be empty.</returns>
    Task<OMTransactionListResponse> GetTransactionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific case, identified by its reference number.
    /// </summary>
    /// <remarks>This method retrieves cases associated with the provided reference number and then fetches
    /// transactions for each case. If no cases are found or the operation is unsuccessful, an empty list is
    /// returned.</remarks>
    /// <param name="caseReferenceNumber">The reference number of the case for which transactions are to be retrieved.  This value cannot be null, empty,
    /// or consist only of whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A list of <see cref="OMTransactionDto"/> objects representing the transactions associated with the specified
    /// case. Returns an empty list if the case reference number is invalid, no cases are found, or no transactions are
    /// associated with the case.</returns>
    Task<OMTransactionListResponse> GetTransactionsForCaseByCaseReferenceNumberAsync(string caseReferenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific interaction and customer identification number.
    /// </summary>
    /// <remarks>This method is intended to be used in scenarios where transactions need to be filtered by
    /// both customer and interaction context. Ensure that the provided identifiers are valid and correspond to existing
    /// records in the system.</remarks>
    /// <param name="customerIdentificationNumber">The unique identification number of the customer. This value cannot be null or empty.</param>
    /// <param name="interactionId">The unique identifier of the interaction for which transactions are being retrieved. This value cannot be null
    /// or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
    /// cref="OMTransactionDto"/> objects representing the transactions associated with the specified customer and
    /// interaction. If no transactions are found, the list will be empty.</returns>
    Task<OMTransactionListResponse> GetTransactionsForInteractionByCustomerIdentificationAsync(string customerIdentificationNumber, string interactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific interaction, identified by its reference number and
    /// ID.
    /// </summary>
    /// <remarks>This method first retrieves interactions associated with the specified reference number and
    /// filters them by the provided interaction ID. If transactions are not already loaded in the interaction data,
    /// they are fetched separately by the case ID.</remarks>
    /// <param name="interactionReferenceNumber">The reference number of the interaction. This value cannot be null, empty, or whitespace.</param>
    /// <param name="interactionId">The unique identifier of the interaction. This value cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A list of <see cref="OMTransactionDto"/> objects representing the transactions associated with the specified
    /// interaction. Returns an empty list if no transactions are found or if the input parameters are invalid.</returns>
    Task<OMTransactionListResponse> GetTransactionsForInteractionByReferenceNumberAsync(string interactionReferenceNumber, string interactionId, CancellationToken cancellationToken = default);
}
