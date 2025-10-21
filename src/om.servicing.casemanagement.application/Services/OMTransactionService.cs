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
    /// Checks whether a transaction with the specified reference number exists in the system.
    /// </summary>
    /// <remarks>If the <paramref name="referenceNumber"/> is null, empty, or consists only of whitespace, the
    /// response will include an error message. If an error occurs during the operation, the response will include a
    /// custom exception with details about the failure.</remarks>
    /// <param name="referenceNumber">The reference number of the transaction to check. This value cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="OMTransactionExistsResponse"/> object containing a boolean value indicating whether the
    /// transaction exists and any associated error messages or exceptions.</returns>
    public async Task<OMTransactionExistsResponse> TransactionExistsWithReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        OMTransactionExistsResponse response = new();

        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            response.SetOrUpdateErrorMessage("Reference number is required.");
            return response;
        }

        try
        {
            IEnumerable<OMTransaction>? omOMTransactions = await _transactionRepository.FindAsync(c => c.ReferenceNumber == referenceNumber, cancellationToken);
            if (omOMTransactions != null && omOMTransactions?.Count() > 0)
            {
                response.Data = true;
                return response;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while checking existence of transaction with reference number '{referenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
        }
        return response;
    }

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
    public async Task<OMTransactionExistsResponse> TransactionExistsWithIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        OMTransactionExistsResponse response = new();

        if (string.IsNullOrWhiteSpace(transactionId))
        {
            response.SetOrUpdateErrorMessage("Transaction Id is required.");
            return response;
        }

        try
        {
            IEnumerable<OMTransaction>? omOMTransactions = await _transactionRepository.FindAsync(c => c.Id == transactionId, cancellationToken);
            if (omOMTransactions != null && omOMTransactions?.Count() > 0)
            {
                response.Data = true;
                return response;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while checking existence of transaction with transaction id '{transactionId}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new ReadPersistenceException(ex, errorMessage));
        }
        return response;
    }

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
    public async Task<OMTransactionCreateResponse> CreateTransactionAsync(OMTransactionDto omTransactionDto, CancellationToken cancellationToken = default)
    {
        OMTransactionCreateResponse response = new();

        if (omTransactionDto == null)
        {
            response.SetOrUpdateErrorMessage("Transaction data is required.");
            return response;
        }

        if (omTransactionDto.Case == null)
        {
            response.SetOrUpdateErrorMessage("Case data for transaction is required.");
            return response;
        }

        CaseChannel channel = EnumUtils.GetEnumValueFromName<CaseChannel>(omTransactionDto.Case!.Channel) ?? CaseChannel.Unknown;
        // default to CustomerServicing for now
        OperationalBusinessSegment operationalBusinessSegment = OperationalBusinessSegment.CustomerServicing;
        omTransactionDto.CreatedDate = DateTime.Now;
        omTransactionDto.Id = UlidUtils.NewUlidString();
        omTransactionDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omTransactionDto.Id, channel, operationalBusinessSegment);

        await EnsureUniqueTransactionIdAndReferenceNumber(omTransactionDto, channel, operationalBusinessSegment, cancellationToken);

        OMTransaction omTransaction = DtoToEntityMapper.ToEntity(omTransactionDto);

        try
        {
            await _transactionRepository.AddAsync(omTransaction, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while attempting to create transaction for case '{omTransaction.Case.Id}' with reference '{omTransaction.Case.ReferenceNumber}'. {ex.Message}";
            _loggingService.LogError(errorMessage, ex);

            response.SetOrUpdateCustomException(new WritePersistenceException(ex, errorMessage));
            return response;
        }

        response.Data.Id = omTransaction.Id;
        response.Data.CaseId = omTransaction.CaseId;
        response.Data.InteractionId = omTransaction.InteractionId;
        response.Data.ReferenceNumber = omTransaction.ReferenceNumber;
        response.Data.CaseReferenceNumber = omTransaction.Case?.ReferenceNumber;
        response.Data.InteractionReferenceNumber = omTransaction.Interaction?.ReferenceNumber;

        return response;
    }

    /// <summary>
    /// Ensures that the specified transaction has a unique transaction ID and reference number.
    /// </summary>
    /// <remarks>This method validates the uniqueness of both the transaction ID and reference number. If
    /// either is found to be non-unique, it regenerates the values and retries the validation process up to a maximum
    /// number of attempts. If uniqueness cannot be ensured after the maximum attempts, an <see
    /// cref="InvalidOperationException"/> is thrown.</remarks>
    /// <param name="omTransactionDto">The transaction data transfer object containing the transaction ID and reference number to validate and
    /// potentially regenerate.</param>
    /// <param name="channel">The channel associated with the transaction, used in generating a new reference number if needed.</param>
    /// <param name="operationalBusinessSegment">The operational business segment associated with the transaction, used in generating a new reference number if
    /// needed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the uniqueness of the transaction ID or reference number cannot be verified due to an unexpected
    /// state, or if a unique value cannot be generated after the maximum number of attempts.</exception>
    private async Task EnsureUniqueTransactionIdAndReferenceNumber(OMTransactionDto omTransactionDto, CaseChannel channel, OperationalBusinessSegment operationalBusinessSegment, CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 10;
        int attempts = 0;

        // ensure that there is no existing interaction with the same id
        while (true)
        {
            var idExistsResponse = await TransactionExistsWithIdAsync(omTransactionDto.Id, cancellationToken);
            if (!idExistsResponse.Success)
            {
                // If we cannot determine existence, stop and surface the issue.
                // We are choosing the throw here because this is an unexpected state that we cannot recover from.
                throw new InvalidOperationException($"Unable to verify transaction id uniqueness: {string.Join("; ", idExistsResponse.ErrorMessages ?? new List<string>())}");
            }

            if (!idExistsResponse.Data)
            {
                break; // id is unique
            }

            // regenerate and retry
            omTransactionDto.Id = UlidUtils.NewUlidString();
            omTransactionDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omTransactionDto.Id, channel, operationalBusinessSegment);

            if (++attempts >= maxAttempts)
            {
                throw new InvalidOperationException($"Unable to generate a unique transaction id after {attempts} attempts.");
            }
        }

        // ensure that there is no existing transaction with the same reference number
        attempts = 0;
        while (true)
        {
            var refExistsResponse = await TransactionExistsWithReferenceNumberAsync(omTransactionDto.ReferenceNumber, cancellationToken);
            if (!refExistsResponse.Success)
            {
                throw new InvalidOperationException($"Unable to verify reference number uniqueness for transaction: {string.Join("; ", refExistsResponse.ErrorMessages ?? new List<string>())}");
            }

            if (!refExistsResponse.Data)
            {
                break; // reference number is unique
            }

            // regenerate and retry
            omTransactionDto.Id = UlidUtils.NewUlidString();
            omTransactionDto.ReferenceNumber = ReferenceNumberGenerator.GenerateReferenceNumber(omTransactionDto.Id, channel, operationalBusinessSegment);

            if (++attempts >= maxAttempts)
            {
                throw new InvalidOperationException($"Unable to generate a unique transaction reference number after {attempts} attempts.");
            }
        }
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
