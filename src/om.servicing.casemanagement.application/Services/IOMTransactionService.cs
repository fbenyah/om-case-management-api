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
    Task<List<OMTransactionDto>> GetTransactionsForCaseByCaseIdAsync(string caseId);

    /// <summary>
    /// Retrieves a list of transactions associated with a specific case, filtered by the customer's identification
    /// number.
    /// </summary>
    /// <param name="customerIdentificationNumber">The identification number of the customer whose transactions are to be retrieved.  This value cannot be null or
    /// empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
    /// cref="OMTransactionDto"/> objects representing the transactions for the specified customer. If no transactions
    /// are found, the list will be empty.</returns>
    Task<List<OMTransactionDto>> GetTransactionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber);

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
    Task<List<OMTransactionDto>> GetTransactionsForInteractionByCustomerIdentificationAsync(string customerIdentificationNumber, string interactionId);
}
