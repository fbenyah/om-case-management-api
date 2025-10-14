using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMCaseService
{
    /// <summary>
    /// Retrieves a list of cases associated with the specified customer.
    /// </summary>
    /// <param name="identityNumber">The unique identity number of the customer whose cases are to be retrieved. This value cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see cref="OMCaseDto"/>
    /// objects representing the cases for the specified customer.  The list will be empty if no cases are found.</returns>
    Task<List<OMCaseDto>> GetCasesForCustomer(string identityNumber);

    /// <summary>
    /// Retrieves a list of cases for a specific customer filtered by the given status.
    /// </summary>
    /// <remarks>This method performs an asynchronous operation to retrieve cases. Ensure that the provided
    /// identity number and status are valid and correspond to existing data.</remarks>
    /// <param name="identityNumber">The unique identifier of the customer whose cases are to be retrieved. Cannot be null or empty.</param>
    /// <param name="status">The status used to filter the cases. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="OMCaseDto"/>
    /// objects matching the specified customer and status. The list will be empty if no matching cases are found.</returns>
    Task<List<OMCaseDto>> GetCasesForCustomerByStatusAsync(string identityNumber, string status);
}
