using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.application.Features.OMInteractions.Queries;

/// <summary>
/// Represents a query to retrieve interactions associated with a specific case, identified by its case ID.
/// </summary>
/// <remarks>This query is used to fetch interaction details for a given case. The case is identified by the <see
/// cref="CaseId"/> property. The response type for this query is <see
/// cref="GetInteractionsForCaseByCaseIdResponse"/>.</remarks>
public class GetInteractionsForCaseByCaseIdQuery : IRequest<GetInteractionsForCaseByCaseIdResponse>
{
    public string CaseId { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of interactions associated with a specific case ID.
/// </summary>
/// <remarks>This response is returned by operations that retrieve interactions for a given case.  The <see
/// cref="Data"> property contains the list of interactions, represented as  <see cref="domain.Dtos.OMInteractionDto">
/// objects. If no interactions are found, the list will be empty.</remarks>
public class GetInteractionsForCaseByCaseIdResponse : ApplicationBaseResponse<List<domain.Dtos.OMInteractionDto>>, IResponse
{
    public GetInteractionsForCaseByCaseIdResponse()
    {
        Data = new List<domain.Dtos.OMInteractionDto>();
    }
}

/// <summary>
/// Handles queries to retrieve interactions associated with a specific case ID.
/// </summary>
/// <remarks>This class is responsible for processing <see cref="GetInteractionsForCaseByCaseIdQuery"/> requests
/// and returning a <see cref="GetInteractionsForCaseByCaseIdResponse"/> containing the interactions related to the
/// specified case ID. It uses the <see cref="Services.IOMInteractionService"/> to fetch the data.</remarks>
public class GetInteractionsForCaseByCaseIdQueryHandler : SharedFeatures, IRequestHandler<GetInteractionsForCaseByCaseIdQuery, GetInteractionsForCaseByCaseIdResponse>
{
    private readonly Services.IOMInteractionService _interactionService;
    public GetInteractionsForCaseByCaseIdQueryHandler
        (
            OM.RequestFramework.Core.Logging.ILoggingService loggingService,
            Services.IOMInteractionService interactionService
        )
        : base(loggingService)
    {
        _interactionService = interactionService;
    }

    /// <summary>
    /// Handles the retrieval of interactions associated with a specific case ID.
    /// </summary>
    /// <param name="request">The query containing the case ID for which interactions are to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="GetInteractionsForCaseByCaseIdResponse"/> containing the list of interactions for the specified
    /// case ID. If the case ID is null, empty, or whitespace, the response will include an error message.</returns>
    public async Task<GetInteractionsForCaseByCaseIdResponse> Handle(GetInteractionsForCaseByCaseIdQuery request, CancellationToken cancellationToken)
    {
        GetInteractionsForCaseByCaseIdResponse response = new();

        if (string.IsNullOrWhiteSpace(request.CaseId))
        {
            response.SetOrUpdateErrorMessage("Case Id is required.");
            return response;
        }

        OMInteractionListResponse omInteractionListResponse = await _interactionService.GetInteractionsForCaseByCaseIdAsync(request.CaseId, TransactionsIncludeNavigationProperties, cancellationToken);
        
        if (!omInteractionListResponse.Success)
        {
            if (omInteractionListResponse.ErrorMessages != null && omInteractionListResponse.ErrorMessages.Any())
            {
                response.SetOrUpdateErrorMessages(omInteractionListResponse.ErrorMessages);
            }

            if (omInteractionListResponse.CustomExceptions != null && omInteractionListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omInteractionListResponse.CustomExceptions);
            }

            return response;
        }

        response.Data = omInteractionListResponse.Data;

        return response;
    }
}
