using MediatR;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.application.Features.OMTransactions.Queries;

/// <summary>
/// Represents a query to retrieve transactions associated with a specific case  based on the customer's identification
/// number.
/// </summary>
/// <remarks>This query is used to fetch transaction details for a case by providing the  customer's unique
/// identification number. The result of the query is returned  as a <see
/// cref="GetTransactionsForCaseByCustomerIdentificationResponse"/>.</remarks>
public class GetTransactionsForCaseByCustomerIdentificationQuery : IRequest<GetTransactionsForCaseByCustomerIdentificationResponse>
{
    public string CustomerIdentificationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of transactions associated with a specific case and customer
/// identification.
/// </summary>
/// <remarks>This response is returned by operations that retrieve transaction data for a given case and customer.
/// The <see cref="Data"/> property contains the list of transactions, which will be empty if no transactions are
/// found.</remarks>
public class GetTransactionsForCaseByCustomerIdentificationResponse : ApplicationBaseResponse<List<domain.Dtos.OMTransactionDto>>, IResponse
{
    public GetTransactionsForCaseByCustomerIdentificationResponse()
    {
        Data = new List<domain.Dtos.OMTransactionDto>();
    }
}

/// <summary>
/// Handles queries to retrieve transactions associated with a specific case based on a customer's identification
/// number.
/// </summary>
/// <remarks>This handler processes the <see cref="GetTransactionsForCaseByCustomerIdentificationQuery"/> and
/// returns a response containing the transactions related to the specified customer identification number. If the
/// customer identification number is not provided, an error message is set in the response.</remarks>
public class GetTransactionsForCaseByCustomerIdentificationQueryHandler : SharedFeatures, IRequestHandler<GetTransactionsForCaseByCustomerIdentificationQuery, GetTransactionsForCaseByCustomerIdentificationResponse>
{
    private readonly Services.IOMTransactionService _transactionService;
    public GetTransactionsForCaseByCustomerIdentificationQueryHandler
        (
            OM.RequestFramework.Core.Logging.ILoggingService loggingService,
            Services.IOMTransactionService transactionService
        )
        : base(loggingService)
    {
        _transactionService = transactionService;
    }

    public async Task<GetTransactionsForCaseByCustomerIdentificationResponse> Handle(GetTransactionsForCaseByCustomerIdentificationQuery request, CancellationToken cancellationToken)
    {
        var response = new GetTransactionsForCaseByCustomerIdentificationResponse();

        if (string.IsNullOrWhiteSpace(request.CustomerIdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Customer identification number is required.");
            return response;
        }

        var transactions = await _transactionService.GetTransactionsForCaseByCustomerIdentificationAsync(request.CustomerIdentificationNumber, cancellationToken);
        response.Data = transactions;

        return response;
    }
}
