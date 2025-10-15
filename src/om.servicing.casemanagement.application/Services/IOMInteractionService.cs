using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMInteractionService
{
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
}
