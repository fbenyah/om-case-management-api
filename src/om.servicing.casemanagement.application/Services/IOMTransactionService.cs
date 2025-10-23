using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.application.Services;

public interface IOMTransactionService
{
    /// <summary>
    /// Retrieves a list of transactions associated with the specified transaction ID.
    /// </summary>
    /// <remarks>This method queries the underlying data store to retrieve transactions associated with the
    /// given  <paramref name="transactionId"/>. If the <paramref name="transactionId"/> is invalid or no transactions 
    /// are found, the response will include an appropriate error message or exception.</remarks>
    /// <param name="transactionId">The unique identifier of the transaction for which associated transactions are to be retrieved.  This parameter
    /// cannot be null, empty, or consist only of whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>An <see cref="OMTransactionListResponse"/> containing the list of associated transactions if found,  or an error
    /// message if no transactions are found or an error occurs during retrieval.</returns>
    Task<OMTransactionListResponse> GetTransactionsForTransactionByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with the specified case ID.
    /// </summary>
    /// <remarks>If an error occurs during the retrieval process, the response will include a custom exception
    /// with details about the failure.</remarks>
    /// <param name="caseId">The unique identifier of the case for which transactions are to be retrieved. Cannot be null, empty, or
    /// whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Optional.</param>
    /// <returns>An <see cref="OMTransactionListResponse"/> containing the list of transactions associated with the specified
    /// case ID. If no transactions are found, the response will include a custom exception indicating the absence of
    /// data.</returns>
    Task<OMTransactionListResponse> GetTransactionsForCaseByCaseIdAsync(string caseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with cases for a given customer identification number.
    /// </summary>
    /// <remarks>This method retrieves cases for the specified customer using their identification number and
    /// then fetches the associated transactions. If an error occurs during the retrieval of cases, the response will
    /// include a custom exception with details about the failure.</remarks>
    /// <param name="customerIdentificationNumber">The unique identification number of the customer. This value cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>An <see cref="OMTransactionListResponse"/> containing the list of transactions associated with the customer's
    /// cases. If the <paramref name="customerIdentificationNumber"/> is invalid, the response will include an error
    /// message.</returns>
    Task<OMTransactionListResponse> GetTransactionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific case reference number.
    /// </summary>
    /// <remarks>This method retrieves cases associated with the provided case reference number and processes
    /// them to obtain the corresponding transactions.  If an error occurs during the retrieval of cases, the response
    /// will include a custom exception with details about the failure.</remarks>
    /// <param name="caseReferenceNumber">The unique reference number of the case for which transactions are to be retrieved.  This parameter cannot be
    /// null, empty, or consist only of whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>An <see cref="OMTransactionListResponse"/> containing the list of transactions associated with the specified
    /// case reference number.  If the case reference number is invalid or an error occurs, the response will include an
    /// appropriate error message or exception.</returns>
    Task<OMTransactionListResponse> GetTransactionsForCaseByCaseReferenceNumberAsync(string caseReferenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific interaction and customer identification number.
    /// </summary>
    /// <remarks>This method retrieves interactions for the specified customer using the customer
    /// identification number and then filters the transactions based on the provided interaction ID. If an error occurs
    /// during the retrieval of interactions, the response will include a custom exception with detailed error
    /// information.</remarks>
    /// <param name="customerIdentificationNumber">The unique identifier for the customer. This parameter cannot be null, empty, or consist only of whitespace.</param>
    /// <param name="interactionId">The unique identifier for the interaction. This parameter cannot be null, empty, or consist only of whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>An <see cref="OMTransactionListResponse"/> containing the list of transactions associated with the specified
    /// interaction and customer identification number. If the input parameters are invalid or an error occurs during
    /// processing, the response will include an appropriate error message or exception.</returns>
    Task<OMTransactionListResponse> GetTransactionsForInteractionByCustomerIdentificationAsync(string customerIdentificationNumber, string interactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific interaction reference number and interaction ID.
    /// </summary>
    /// <remarks>This method retrieves interactions for the specified case reference number and processes them
    /// to generate a list of transactions. If an error occurs while retrieving interactions, the response will include
    /// a custom exception with details about the failure.</remarks>
    /// <param name="interactionReferenceNumber">The reference number of the interaction. This value cannot be null, empty, or whitespace.</param>
    /// <param name="interactionId">The unique identifier of the interaction. This value cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>An <see cref="OMTransactionListResponse"/> containing the list of transactions associated with the specified
    /// interaction reference number and interaction ID. If the input parameters are invalid or an error occurs during
    /// processing, the response will include an appropriate error message or exception.</returns>
    Task<OMTransactionListResponse> GetTransactionsForInteractionByReferenceNumberAsync(string interactionReferenceNumber, string interactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a transaction with the specified reference number exists in the system.
    /// </summary>
    /// <remarks>If the <paramref name="referenceNumber"/> is null, empty, or consists only of whitespace, the
    /// response will include an error message. If an error occurs during the operation, the response will include a
    /// custom exception with details about the failure.</remarks>
    /// <param name="referenceNumber">The reference number of the transaction to check. This value cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="OMTransactionExistsResponse"/> object containing a boolean value indicating whether the
    /// transaction exists and any associated error messages or exceptions.</returns>
    Task<OMTransactionExistsResponse> TransactionExistsWithReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously checks whether a transaction with the specified ID exists in the system.
    /// </summary>
    /// <remarks>If the <paramref name="transactionId"/> is null, empty, or consists only of whitespace, the
    /// response will include an error message. If an exception occurs during the operation, the response will include a
    /// custom exception with details about the error.</remarks>
    /// <param name="transactionId">The unique identifier of the transaction to check. Cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="OMTransactionExistsResponse"/> object containing a boolean value indicating whether the
    /// transaction exists and any associated error messages or exceptions.</returns>
    Task<OMTransactionExistsResponse> TransactionExistsWithIdAsync(string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new transaction asynchronously based on the provided transaction data.
    /// </summary>
    /// <remarks>This method validates the provided transaction data and ensures that the transaction ID and
    /// reference number are unique. If the transaction data or associated case data is missing, an error message is set
    /// in the response. In case of a persistence error, a custom exception is logged and included in the
    /// response.</remarks>
    /// <param name="omTransactionDto">The transaction data transfer object containing the details of the transaction to be created. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Optional.</param>
    /// <returns>A <see cref="OMTransactionCreateResponse"/> containing the result of the transaction creation,  including the
    /// generated transaction ID, reference numbers, and any error messages if the operation fails.</returns>
    Task<OMTransactionCreateResponse> CreateTransactionAsync(OMTransactionDto omTransactionDto, CancellationToken cancellationToken = default);
}
