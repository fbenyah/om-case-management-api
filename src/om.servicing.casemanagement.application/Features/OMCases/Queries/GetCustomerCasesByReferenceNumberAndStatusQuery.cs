using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Extensions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMCases.Queries;

/// <summary>
/// Represents a query to retrieve customer cases based on their reference number and status.
/// </summary>
/// <remarks>This query is used to filter customer cases by a specific reference number and status. The
/// result of the query is returned as a <see
/// cref="GetCustomerCasesByReferenceNumberAndStatusResponse"/>.</remarks>
public class GetCustomerCasesByReferenceNumberAndStatusQuery : IRequest<GetCustomerCasesByReferenceNumberAndStatusResponse>
{
    public CaseStatus Status { get; set; } = CaseStatus.Unknown;
    public string ReferenceNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of customer cases filtered by reference number and status.
/// </summary>
/// <remarks>This response is returned by operations that retrieve customer cases based on specific reference
/// numbers and statuses. The <see cref="Data"> property contains the resulting list of cases, which will be empty if no
/// matching cases are found.</remarks>
public class GetCustomerCasesByReferenceNumberAndStatusResponse : ApplicationBaseResponse<List<domain.Dtos.OMCaseDto>>, IResponse
{
    public GetCustomerCasesByReferenceNumberAndStatusResponse()
    {
        Data = new List<domain.Dtos.OMCaseDto>();
    }
}

/// <summary>
/// Handles queries to retrieve customer cases based on an reference number and case status.
/// </summary>
/// <remarks>This handler processes a query to fetch customer cases that match the specified reference number
/// and status. It validates the input parameters and interacts with the case service to retrieve the data.</remarks>
public class GetCustomerCasesByReferenceNumberAndStatusQueryHandler : SharedFeatures, IRequestHandler<GetCustomerCasesByReferenceNumberAndStatusQuery, GetCustomerCasesByReferenceNumberAndStatusResponse>
{
    private readonly Services.IOMCaseService _caseService;

    public GetCustomerCasesByReferenceNumberAndStatusQueryHandler
        (
            ILoggingService loggingService,
            Services.IOMCaseService caseService
        )
        : base(loggingService)
    {
        _caseService = caseService;
    }

    /// <summary>
    /// Handles the query to retrieve customer cases based on the provided reference number and case status.
    /// </summary>
    /// <remarks>This method validates the input parameters and retrieves the customer cases asynchronously.
    /// If the reference number or status is missing or empty, the response will include an appropriate error
    /// message.</remarks>
    /// <param name="request">The query containing the reference number and case status used to filter the customer cases. Both
    /// parameters must be non-empty strings.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="GetCustomerCasesByReferenceNumberAndStatusResponse"/> containing the list of customer cases
    /// that match the specified reference number and status. If the input parameters are invalid, the response
    /// will include an error message.</returns>
    public async Task<GetCustomerCasesByReferenceNumberAndStatusResponse> Handle(GetCustomerCasesByReferenceNumberAndStatusQuery request, CancellationToken cancellationToken)
    {
        GetCustomerCasesByReferenceNumberAndStatusResponse response = new();

        if (string.IsNullOrWhiteSpace(request.ReferenceNumber))
        {
            response.SetOrUpdateErrorMessage("Reference number is required.");
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Status.GetDescription()))
        {
            response.SetOrUpdateErrorMessage("Status of case(s) is required.");
            return response;
        }

        OMCaseListResponse omCaseListResponse = await _caseService.GetCasesForCustomerByReferenceNumberAndStatusAsync(request.ReferenceNumber, request.Status.GetDescription(), InteractionsTransactionsIncludeNavigationProperties, cancellationToken);
        
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
