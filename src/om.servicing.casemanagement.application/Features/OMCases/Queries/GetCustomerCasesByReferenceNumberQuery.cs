using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMCases.Queries;

/// <summary>
/// Represents a query to retrieve customer cases associated with a specific reference number.
/// </summary>
/// <remarks>This query is used to fetch customer case data based on the provided reference number. The
/// result of the query is returned as a <see cref="GetCustomerCasesByReferenceNumberResponse"/>.</remarks>
public class GetCustomerCasesByReferenceNumberQuery : IRequest<GetCustomerCasesByReferenceNumberResponse>
{
    public string ReferenceNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of customer cases associated with a specific reference number.
/// </summary>
/// <remarks>This response includes a collection of <see cref="domain.Dtos.OMCaseDto"/> objects, which provide
/// detailed information about each case. The <see cref="Data"/> property is initialized to an empty list by
/// default.</remarks>
public class GetCustomerCasesByReferenceNumberResponse : ApplicationBaseResponse<List<domain.Dtos.OMCaseDto>>, IResponse
{
    public GetCustomerCasesByReferenceNumberResponse()
    {
        Data = new List<domain.Dtos.OMCaseDto>();
    }
}

/// <summary>
/// Handles queries to retrieve customer cases based on an reference number.
/// </summary>
/// <remarks>This handler processes a <see cref="GetCustomerCasesByReferenceNumberQuery"/> and returns a <see
/// cref="GetCustomerCasesByReferenceNumberResponse"/> containing the customer cases associated with the provided
/// reference number. If the reference number is null or whitespace, an error message is set in the
/// response.</remarks>
public class GetCustomerCasesByReferenceNumberQueryHandler : SharedFeatures, IRequestHandler<GetCustomerCasesByReferenceNumberQuery, GetCustomerCasesByReferenceNumberResponse>
{
    private readonly Services.IOMCaseService _caseService;

    public GetCustomerCasesByReferenceNumberQueryHandler
        (
            ILoggingService loggingService,
            Services.IOMCaseService caseService
        )
        : base(loggingService)
    {
        _caseService = caseService;
    }

    /// <summary>
    /// Handles the query to retrieve customer cases based on the provided reference number.
    /// </summary>
    /// <remarks>The reference number provided in the query must not be null, empty, or consist only of
    /// whitespace. If the reference number is invalid, the response will include an appropriate error
    /// message.</remarks>
    /// <param name="request">The query containing the reference number used to retrieve customer cases.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="GetCustomerCasesByReferenceNumberResponse"/> with the list of customer cases if the identification
    /// number is valid, or an error message if the reference number is missing or invalid.</returns>
    public async Task<GetCustomerCasesByReferenceNumberResponse> Handle(GetCustomerCasesByReferenceNumberQuery request, CancellationToken cancellationToken)
    {
        GetCustomerCasesByReferenceNumberResponse response = new ();

        if (string.IsNullOrWhiteSpace(request.ReferenceNumber))
        {
            response.SetOrUpdateErrorMessage("Reference number is required.");
            return response;
        }
        
        OMCaseListResponse omCaseListResponse = await _caseService.GetCasesForCustomerByReferenceNumberAsync(request.ReferenceNumber, InteractionsTransactionsIncludeNavigationProperties, cancellationToken);
        
        if (!omCaseListResponse.Success)
        {
            if (omCaseListResponse.ErrorMessages != null && omCaseListResponse.ErrorMessages.Any())
            {
                response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);
            }

            if (omCaseListResponse.CustomExceptions != null && omCaseListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }

            return response;
        }

        response.Data = omCaseListResponse.Data;

        return response;
    }
}
