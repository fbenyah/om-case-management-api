using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.application.Features.OMTransactions.Queries;

/// <summary>
/// Represents a query to retrieve transactions associated with a specific interaction and customer identification
/// number.
/// </summary>
/// <remarks>This query is used to fetch transaction details for a given interaction ID and customer
/// identification number. The result of the query is returned as a <see
/// cref="GetTransactionsForInteractionByCustomerIdentificationResponse"/>.</remarks>
public class GetTransactionsForInteractionByCustomerIdentificationQuery : IRequest<GetTransactionsForInteractionByCustomerIdentificationResponse>
{
    public string InteractionId { get; set; } = string.Empty;
    public string CustomerIdentificationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a collection of transactions associated with a specific customer interaction.
/// </summary>
/// <remarks>This response is typically used to return a list of transactions related to a customer interaction, 
/// identified by customer-specific criteria. The <see cref="Data"/> property contains the collection of  transactions,
/// which is initialized as an empty list by default.</remarks>
public class GetTransactionsForInteractionByCustomerIdentificationResponse : ApplicationBaseResponse<List<domain.Dtos.OMTransactionDto>>, IResponse
{
    public GetTransactionsForInteractionByCustomerIdentificationResponse()
    {
        Data = new List<domain.Dtos.OMTransactionDto>();
    }
}

/// <summary>
/// Handles queries to retrieve transactions associated with a specific interaction and customer identification number.
/// </summary>
/// <remarks>This handler processes the <see cref="GetTransactionsForInteractionByCustomerIdentificationQuery"/>
/// and returns a response containing the transactions that match the provided interaction ID and customer
/// identification number.</remarks>
public class GetTransactionsForInteractionByCustomerIdentificationQueryHandler : SharedFeatures, IRequestHandler<GetTransactionsForInteractionByCustomerIdentificationQuery, GetTransactionsForInteractionByCustomerIdentificationResponse>
{
    private readonly Services.IOMTransactionService _transactionService;
    public GetTransactionsForInteractionByCustomerIdentificationQueryHandler
        (
            OM.RequestFramework.Core.Logging.ILoggingService loggingService,
            Services.IOMTransactionService transactionService
        )
        : base(loggingService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Handles the query to retrieve transactions associated with a specific interaction and customer identification
    /// number.
    /// </summary>
    /// <remarks>This method validates the input parameters and retrieves the transactions using the provided
    /// interaction ID and customer identification number. If the input parameters are invalid, the response will
    /// include an appropriate error message.</remarks>
    /// <param name="request">The query containing the customer identification number and interaction ID. Both values must be provided.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="GetTransactionsForInteractionByCustomerIdentificationResponse"/> containing the transactions
    /// associated with the specified interaction and customer identification number. If either the customer
    /// identification number or interaction ID is missing, the response will include an error message.</returns>
    public async Task<GetTransactionsForInteractionByCustomerIdentificationResponse> Handle(GetTransactionsForInteractionByCustomerIdentificationQuery request, CancellationToken cancellationToken)
    {
        var response = new GetTransactionsForInteractionByCustomerIdentificationResponse();

        if (string.IsNullOrWhiteSpace(request.CustomerIdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Customer identification number is required.");
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.InteractionId))
        {
            response.SetOrUpdateErrorMessage("Interaction ID is required.");
            return response;
        }

        OMTransactionListResponse omTransactionListResponse = await _transactionService.GetTransactionsForInteractionByCustomerIdentificationAsync(request.CustomerIdentificationNumber, request.InteractionId, null, cancellationToken);

        if (!omTransactionListResponse.Success)
        {
            if (omTransactionListResponse.ErrorMessages != null && omTransactionListResponse.ErrorMessages.Any())
            {
                response.SetOrUpdateErrorMessages(omTransactionListResponse.ErrorMessages);
            }

            if (omTransactionListResponse.CustomExceptions != null && omTransactionListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omTransactionListResponse.CustomExceptions);
            }

            return response;
        }

        response.Data = omTransactionListResponse.Data;

        return response;
    }
}
