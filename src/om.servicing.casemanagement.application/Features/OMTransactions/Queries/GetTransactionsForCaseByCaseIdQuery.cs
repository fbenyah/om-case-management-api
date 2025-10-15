using MediatR;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.application.Features.OMTransactions.Queries;

/// <summary>
/// Represents a query to retrieve transactions associated with a specific case, identified by its case ID.
/// </summary>
/// <remarks>This query is used to request transaction data for a specific case. The case is identified by the
/// <see cref="CaseId"/> property. The response to this query is of type <see
/// cref="GetTransactionsForCaseByCaseIdResponse"/>.</remarks>
public class GetTransactionsForCaseByCaseIdQuery : IRequest<GetTransactionsForCaseByCaseIdResponse>
{
    public string CaseId { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of transactions associated with a specific case ID.
/// </summary>
/// <remarks>This response is returned by operations that retrieve transaction data for a given case.  The <see
/// cref="Data"/> property contains the list of transactions, represented as  <see cref="domain.Dtos.OMTransactionDto"/>
/// objects. If no transactions are found, the list will be empty.</remarks>
public class GetTransactionsForCaseByCaseIdResponse : ApplicationBaseResponse<List<domain.Dtos.OMTransactionDto>>, IResponse
{
    public GetTransactionsForCaseByCaseIdResponse()
    {
        Data = new List<domain.Dtos.OMTransactionDto>();
    }
}

/// <summary>
/// Handles queries to retrieve transactions associated with a specific case ID.
/// </summary>
/// <remarks>This class is responsible for processing <see cref="GetTransactionsForCaseByCaseIdQuery"/> requests
/// and returning a <see cref="GetTransactionsForCaseByCaseIdResponse"/> containing the transactions linked to the
/// specified case ID. It relies on the <see cref="Services.IOMTransactionService"/> to fetch the transaction
/// data.</remarks>
public class GetTransactionsForCaseByCaseIdQueryHandler : SharedFeatures, IRequestHandler<GetTransactionsForCaseByCaseIdQuery, GetTransactionsForCaseByCaseIdResponse>
{
    private readonly Services.IOMTransactionService _transactionService;
    public GetTransactionsForCaseByCaseIdQueryHandler
        (
            OM.RequestFramework.Core.Logging.ILoggingService loggingService,
            Services.IOMTransactionService transactionService
        )
        : base(loggingService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Handles the query to retrieve transactions associated with a specific case ID.
    /// </summary>
    /// <param name="request">The query containing the case ID for which transactions are to be retrieved. The <see
    /// cref="GetTransactionsForCaseByCaseIdQuery.CaseId"/> property must not be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="GetTransactionsForCaseByCaseIdResponse"/> containing the list of transactions associated with the
    /// specified case ID. If the case ID is invalid, the response will include an error message.</returns>
    public async Task<GetTransactionsForCaseByCaseIdResponse> Handle(GetTransactionsForCaseByCaseIdQuery request, CancellationToken cancellationToken)
    {
        GetTransactionsForCaseByCaseIdResponse response = new();
        if (string.IsNullOrWhiteSpace(request.CaseId))
        {
            response.SetOrUpdateErrorMessage("The Case Id is required.");
            return response;
        }

        List<domain.Dtos.OMTransactionDto> transactions = await _transactionService.GetTransactionsForCaseByCaseIdAsync(request.CaseId, cancellationToken);
        response.Data = transactions;

        return response;
    }
}
