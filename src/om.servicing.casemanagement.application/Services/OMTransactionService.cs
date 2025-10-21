using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Services;

public class OMTransactionService : BaseService, IOMTransactionService
{
    private readonly IOMCaseService _omCaseService;
    private readonly IOMInteractionService _omInteractionService;
    private readonly IGenericRepository<OMTransaction> _transactionRepository;

    public OMTransactionService(
        IOMCaseService oMCaseService,
        ILoggingService loggingService,
        IOMInteractionService omInteractionService,
        IGenericRepository<OMTransaction> transactionRepository
        ) : base(loggingService)
    {
        _omCaseService = oMCaseService;
        _omInteractionService = omInteractionService;
        _transactionRepository = transactionRepository;
    }

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
    public async Task<OMTransactionListResponse> GetTransactionsForCaseByCaseIdAsync(string caseId, CancellationToken cancellationToken = default)
    {
        OMTransactionListResponse response = new();

        if (string.IsNullOrWhiteSpace(caseId))
        {
            response.SetOrUpdateErrorMessage("Case Id is required.");
            return response;
        }

        try
        {
            IEnumerable<OMTransaction> omTransactions = await _transactionRepository.FindAsync(t => t.CaseId == caseId);

            if (omTransactions == null || !omTransactions.Any())
            {
                response.SetOrUpdateCustomException(new NotFoundException($"No transactions found for case id '{caseId}'."));
                return response;
            }

            response.Data = OMTransactionUtilities.ReturnTransactionDtoList(omTransactions);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving transactions with case id '{caseId}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }        

        return response;
    }

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
    public async Task<OMTransactionListResponse> GetTransactionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber, CancellationToken cancellationToken = default)
    {
        OMTransactionListResponse response = new();

        if (string.IsNullOrWhiteSpace(customerIdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Customer Identification Number is required.");
            return response;
        }

        OMCaseListResponse omCaseListResponse;

        try
        {
            omCaseListResponse = await _omCaseService.GetCasesForCustomerByIdentificationNumberAsync(customerIdentificationNumber);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving cases for customer by identification number '{customerIdentificationNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        await GetListOfTransactions(omCaseListResponse, customerIdentificationNumber, response);
        return response;
    }

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
    public async Task<OMTransactionListResponse> GetTransactionsForCaseByCaseReferenceNumberAsync(string caseReferenceNumber, CancellationToken cancellationToken = default)
    {
        OMTransactionListResponse response = new();

        if (string.IsNullOrWhiteSpace(caseReferenceNumber))
        {
            response.SetOrUpdateErrorMessage("Case Reference Number is required.");
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

        await GetListOfTransactions(omCaseListResponse, caseReferenceNumber, response);
        return response;
    }

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
    public async Task<OMTransactionListResponse> GetTransactionsForInteractionByCustomerIdentificationAsync(string customerIdentificationNumber, string interactionId, CancellationToken cancellationToken = default)
    {
        OMTransactionListResponse response = new();

        if (string.IsNullOrWhiteSpace(customerIdentificationNumber) 
            || string.IsNullOrWhiteSpace(interactionId))
        {
            response.SetOrUpdateErrorMessage("Customer Identification Number and Interaction Id are required.");
            return response;
        }

        OMInteractionListResponse omInteractionListResponse;

        try
        {
            omInteractionListResponse = await _omInteractionService.GetInteractionsForCaseByCustomerIdentificationAsync(customerIdentificationNumber);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving interactions for case for customer by customer identification number '{customerIdentificationNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }      

        await GetListOfTransactions(interactionId, omInteractionListResponse, customerIdentificationNumber, response);
        return response;
    }

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
    public async Task<OMTransactionListResponse> GetTransactionsForInteractionByReferenceNumberAsync(string interactionReferenceNumber, string interactionId, CancellationToken cancellationToken = default)
    {
        OMTransactionListResponse response = new();

        if (string.IsNullOrWhiteSpace(interactionReferenceNumber)
            || string.IsNullOrWhiteSpace(interactionId))
        {
            response.SetOrUpdateErrorMessage("Interaction Reference Number and Interaction Id are required.");
            return response;
        }

        OMInteractionListResponse omInteractionListResponse;

        try
        {
            omInteractionListResponse = await _omInteractionService.GetInteractionsForCaseByCaseReferenceNumberAsync(interactionReferenceNumber);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while retrieving interactions for case for customer by interaction reference number '{interactionReferenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
            return response;
        }

        await GetListOfTransactions(interactionId, omInteractionListResponse, interactionReferenceNumber, response);
        return response;
    }

    /// <summary>
    /// Retrieves a filtered list of transactions associated with a specific interaction ID and updates the response
    /// object accordingly.
    /// </summary>
    /// <remarks>This method processes the provided interaction list response to filter interactions by the
    /// specified interaction ID.  If transactions are not already loaded in the interaction data, it attempts to fetch
    /// them asynchronously by case ID.  The method updates the <paramref name="response"/> object with the resulting
    /// transactions, error messages, or exceptions as appropriate.</remarks>
    /// <param name="interactionId">The unique identifier of the interaction to filter transactions by.</param>
    /// <param name="omInteractionListResponse">The response object containing a list of interactions and their associated data.</param>
    /// <param name="caseSearchParameter">The search parameter used to identify the case associated with the interactions.</param>
    /// <param name="response">The response object to be updated with the resulting list of transactions or error information.</param>
    /// <returns></returns>
    private async Task GetListOfTransactions(string interactionId, OMInteractionListResponse omInteractionListResponse, string caseSearchParameter, OMTransactionListResponse response)
    {
        if (!omInteractionListResponse.Success)
        {
            response.SetOrUpdateErrorMessages(omInteractionListResponse.ErrorMessages);

            if (omInteractionListResponse.CustomExceptions != null)
            {
                response.SetOrUpdateCustomExceptions(omInteractionListResponse.CustomExceptions);
            }

            return;
        }

        if (omInteractionListResponse.Data == null || !omInteractionListResponse.Data.Any())
        {
            response.SetOrUpdateCustomException(new NotFoundException($"No interactions found with '{caseSearchParameter}'."));
            return;
        }

        omInteractionListResponse.Data = omInteractionListResponse.Data.Where(i => i.Id == interactionId)?.ToList();

        if (omInteractionListResponse.Data == null || !omInteractionListResponse.Data.Any())
        {
            response.SetOrUpdateCustomException(new NotFoundException($"No interactions found with interaction id '{interactionId}' after filtering list of interactions."));
            return;
        }

        List<OMTransactionDto> allTransactionsDto = new();

        foreach (OMInteractionDto omInteractionDto in omInteractionListResponse.Data)
        {
            if (omInteractionDto.Transactions != null && omInteractionDto.Transactions.Any())
            {
                allTransactionsDto.AddRange(omInteractionDto.Transactions);
                continue;
            }
            
            try
            {
                // If transactions are not already loaded in the interaction DTO, fetch them by case ID
                OMTransactionListResponse transactionListResponse = await GetTransactionsForCaseByCaseIdAsync(omInteractionDto.Case?.Id);

                if (!transactionListResponse.Success)
                {
                    response.SetOrUpdateErrorMessages(transactionListResponse.ErrorMessages);

                    if (transactionListResponse.CustomExceptions != null)
                    {
                        response.SetOrUpdateCustomExceptions(transactionListResponse.CustomExceptions);
                    }

                    return;
                }

                if (transactionListResponse.Data == null || !transactionListResponse.Data.Any())
                {
                    response.SetOrUpdateCustomException(new NotFoundException($"No transactions found for case with id '{omInteractionDto.Case?.Id}'."));
                    return;
                }

                allTransactionsDto.AddRange(transactionListResponse.Data);
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred while retrieving transactions for case by case id '{omInteractionDto.Case?.Id}'. {ex.Message}";
                _loggingService.LogError(errorMessage, ex);

                response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
                return;
            }
        }

        response.Data = allTransactionsDto;
        return;
    }

    /// <summary>
    /// Retrieves a list of transactions associated with cases based on the provided search parameter.
    /// </summary>
    /// <remarks>This method processes a list of cases and retrieves transactions for each case. If any errors
    /// occur during the process,  they are logged and added to the <paramref name="response"/> object. If no cases or
    /// transactions are found, appropriate  exceptions are set in the response.</remarks>
    /// <param name="omCaseListResponse">The response containing the list of cases to process. Must indicate success and contain valid data.</param>
    /// <param name="caseSearchParameter">The search parameter used to identify cases. Used for error reporting if no cases are found.</param>
    /// <param name="response">The response object to populate with the resulting transactions or error information.</param>
    /// <returns></returns>
    private async Task GetListOfTransactions(OMCaseListResponse omCaseListResponse, string caseSearchParameter, OMTransactionListResponse response)
    {
        if (!omCaseListResponse.Success)
        {
            response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);

            if (omCaseListResponse.CustomExceptions != null)
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }

            return;
        }

        if (omCaseListResponse.Data == null || !omCaseListResponse.Data.Any())
        {
            response.SetOrUpdateCustomException(new NotFoundException($"No cases found with '{caseSearchParameter}'."));
            return;
        }

        List<OMTransactionDto> allTransactionsDto = new();

        foreach (OMCaseDto omCaseDto in omCaseListResponse.Data)
        {
            OMTransactionListResponse transactionForCaseResponse;

            try
            {
                transactionForCaseResponse = await GetTransactionsForCaseByCaseIdAsync(omCaseDto.Id);
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred while retrieving transactions for case by case id '{omCaseDto.Id}'. {ex.Message}";
                _loggingService.LogError(errorMessage, ex);

                response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
                return;
            }

            if (!transactionForCaseResponse.Success)
            {
                response.SetOrUpdateErrorMessages(transactionForCaseResponse.ErrorMessages);

                if (transactionForCaseResponse.CustomExceptions != null)
                {
                    response.SetOrUpdateCustomExceptions(transactionForCaseResponse.CustomExceptions);
                }

                return;
            }

            if (transactionForCaseResponse.Data == null || !transactionForCaseResponse.Data.Any())
            {
                response.SetOrUpdateCustomException(new NotFoundException($"No transactions found for case with id '{omCaseDto.Id}'."));
                return;
            }

            allTransactionsDto.AddRange(transactionForCaseResponse.Data);
        }

        response.Data = allTransactionsDto;
        return;
    }
}
