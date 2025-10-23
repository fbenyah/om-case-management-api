using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Exceptions.Client;
using om.servicing.casemanagement.domain.Mappings;
using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMTransactionUtilities
{
    /// <summary>
    /// Converts a collection of <see cref="OMTransaction"/> objects to a list of <see cref="OMTransactionDto"/>
    /// objects.
    /// </summary>
    /// <param name="omTransactions">The collection of <see cref="OMTransaction"/> objects to convert. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMTransactionDto"/> objects. Returns an empty list if <paramref name="omTransactions"/> is
    /// null or contains no elements.</returns>
    public static List<OMTransactionDto> ReturnTransactionDtoList(IEnumerable<OMTransaction> omTransactions)
    {
        List<OMTransactionDto> transactionDtoList = new List<OMTransactionDto>();

        if (omTransactions == null || !omTransactions.Any())
        {
            return transactionDtoList;
        }

        foreach (OMTransaction omTransaction in omTransactions)
        {
            transactionDtoList.Add(EntityToDtoMapper.ToDto(omTransaction));
        }

        return transactionDtoList;
    }

    /// <summary>
    /// Converts a list of transaction DTOs to a list of transaction entities.
    /// </summary>
    /// <param name="transactionDtoList">A list of <see cref="OMTransactionDto"/> objects to be converted.  If the list is null or empty, an empty list
    /// is returned.</param>
    /// <returns>A list of <see cref="OMTransaction"/> objects converted from the provided DTOs.  Returns an empty list if
    /// <paramref name="transactionDtoList"/> is null or contains no elements.</returns>
    public static List<OMTransaction> ReturnTransactionList(List<OMTransactionDto> transactionDtoList)
    {
        List<OMTransaction> transactionList = new List<OMTransaction>();

        if (transactionDtoList == null || !transactionDtoList.Any())
        {
            return transactionList;
        }

        foreach (OMTransactionDto omTransactionDto in transactionDtoList)
        {
            transactionList.Add(DtoToEntityMapper.ToEntity(omTransactionDto));
        }

        return transactionList;
    }

    /// <summary>
    /// Determines whether a transaction is eligible for creating other entities based on the provided transaction ID.
    /// </summary>
    /// <remarks>This method retrieves transaction details using the provided transaction ID and evaluates the
    /// results to determine eligibility: <list type="bullet"> <item>If the transaction retrieval fails, the response is
    /// updated with error messages and custom exceptions.</item> <item>If no transactions are found, the response is
    /// updated with an error message indicating the absence of transactions.</item> <item>If multiple transactions are
    /// found, the response is updated with an error message and a <see cref="ConflictException"/>.</item> </list> The
    /// method does not return a value but updates the provided <paramref name="response"/> object with the results of
    /// the evaluation.</remarks>
    /// <typeparam name="TResponse">The type of the response object, which must inherit from <see cref="BaseFluentValidationError"/>.</typeparam>
    /// <param name="transactionId">The unique identifier of the transaction to evaluate. Cannot be null or empty.</param>
    /// <param name="omTransactionListResponse">An object that will hold the response from the transaction service, including transaction data and any error
    /// messages.</param>
    /// <param name="response">The response object to update with validation errors, custom exceptions, or other relevant information based on
    /// the transaction's eligibility.</param>
    /// <param name="transactionService">The service used to retrieve transaction details by transaction ID. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns></returns>
    public async static Task DetermineIfTransactionIsEligibleForOtherEntityCreation<TResponse>(string transactionId, OMTransactionListResponse omTransactionListResponse, TResponse response, Services.IOMTransactionService transactionService, CancellationToken cancellationToken)
        where TResponse : BaseFluentValidationError
    {
        omTransactionListResponse = await transactionService.GetTransactionsForTransactionByTransactionIdAsync(transactionId, cancellationToken);

        if (!omTransactionListResponse.Success)
        {

            response.SetOrUpdateErrorMessages(omTransactionListResponse.ErrorMessages);

            if (omTransactionListResponse.CustomExceptions != null && omTransactionListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omTransactionListResponse.CustomExceptions);
            }
        }

        if (omTransactionListResponse.Data == null || omTransactionListResponse.Data.Count == 0)
        {
            response.SetOrUpdateErrorMessage($"No transaction found for Transaction Id: {transactionId}");
            response.SetOrUpdateErrorMessages(omTransactionListResponse.ErrorMessages);

            if (omTransactionListResponse.CustomExceptions != null && omTransactionListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omTransactionListResponse.CustomExceptions);
            }
        }

        if (omTransactionListResponse.Data.Count > 1)
        {
            string errorMessage = $"Multiple transactions found for Transaction Id: {transactionId}";
            response.SetOrUpdateErrorMessage(errorMessage);
            response.SetOrUpdateCustomException(new ConflictException(errorMessage));
        }
    }
}
