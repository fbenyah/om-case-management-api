using MediatR;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.application.Features.OMInteractions.Queries;

/// <summary>
/// Represents a query to retrieve interactions for a specific case based on the customer's identification number.
/// </summary>
/// <remarks>This query is used to fetch interactions associated with a case by providing the customer's unique
/// identification number. The result of the query is returned as a <see
/// cref="GetInteractionsForCaseByCustomerIdentificationResponse"/>.</remarks>
public class GetInteractionsForCaseByCustomerIdentificationQuery : IRequest<GetInteractionsForCaseByCustomerIdentificationResponse>
{
      public string CustomerIdentificationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of interactions associated with a case,  retrieved based on customer
/// identification.
/// </summary>
/// <remarks>This response includes a collection of interaction details encapsulated in  <see
/// cref="domain.Dtos.OMInteractionDto"/> objects. The <see cref="Data"/> property  is initialized to an empty list by
/// default.</remarks>
public class GetInteractionsForCaseByCustomerIdentificationResponse : ApplicationBaseResponse<List<domain.Dtos.OMInteractionDto>>, IResponse
{
    public GetInteractionsForCaseByCustomerIdentificationResponse()
    {
        Data = new List<domain.Dtos.OMInteractionDto>();
    }
}

/// <summary>
/// Handles queries to retrieve interactions for a case based on a customer's identification number.
/// </summary>
/// <remarks>This handler processes the <see cref="GetInteractionsForCaseByCustomerIdentificationQuery"/> and
/// returns a response containing a list of interactions associated with the specified customer identification number.
/// If the customer identification number is null, empty, or whitespace, an error message is set in the
/// response.</remarks>
public class GetInteractionsForCaseByCustomerIdentificationQueryHandler : SharedFeatures, IRequestHandler<GetInteractionsForCaseByCustomerIdentificationQuery, GetInteractionsForCaseByCustomerIdentificationResponse>
{
    private readonly Services.IOMInteractionService _interactionService;
    public GetInteractionsForCaseByCustomerIdentificationQueryHandler
        (
            OM.RequestFramework.Core.Logging.ILoggingService loggingService,
            Services.IOMInteractionService interactionService
        )
        : base(loggingService)
    {
        _interactionService = interactionService;
    }

    /// <summary>
    /// Handles the query to retrieve interactions for a case based on the provided customer identification number.
    /// </summary>
    /// <param name="request">The query containing the customer identification number used to retrieve interactions. The <see
    /// cref="GetInteractionsForCaseByCustomerIdentificationQuery.CustomerIdentificationNumber"/> property must not be
    /// null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="GetInteractionsForCaseByCustomerIdentificationResponse"/> containing the list of interactions
    /// associated with the specified customer identification number. If the customer identification number is invalid,
    /// the response will include an error message.</returns>
    public async Task<GetInteractionsForCaseByCustomerIdentificationResponse> Handle(GetInteractionsForCaseByCustomerIdentificationQuery request, CancellationToken cancellationToken)
    {
        GetInteractionsForCaseByCustomerIdentificationResponse response = new();

        if (string.IsNullOrWhiteSpace(request.CustomerIdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Customer Identification Number is required.");
            return response;
        }

        List<domain.Dtos.OMInteractionDto> omInteractions = await _interactionService.GetInteractionsForCaseByCustomerIdentificationAsync(request.CustomerIdentificationNumber);
        response.Data = omInteractions;

        return response;
    }
}
