using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMCases.Queries;

/// <summary>
/// Represents a query to retrieve customer cases associated with a specific identification number.
/// </summary>
/// <remarks>This query is used to fetch customer case data based on the provided identification number. The
/// result of the query is returned as a <see cref="GetCustomerCasesByIdentificationNumberResponse"/>.</remarks>
public class GetCustomerCasesByIdentificationNumberQuery : IRequest<GetCustomerCasesByIdentificationNumberResponse>
{
    public string IdentificationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of customer cases associated with a specific identification number.
/// </summary>
/// <remarks>This response includes a collection of <see cref="domain.Dtos.OMCaseDto"/> objects, which provide
/// detailed information about each case. The <see cref="Data"/> property is initialized to an empty list by
/// default.</remarks>
public class GetCustomerCasesByIdentificationNumberResponse : ApplicationBaseResponse<List<domain.Dtos.OMCaseDto>>, IResponse
{
    public GetCustomerCasesByIdentificationNumberResponse()
    {
        Data = new List<domain.Dtos.OMCaseDto>();
    }
}

/// <summary>
/// Handles queries to retrieve customer cases based on an identification number.
/// </summary>
/// <remarks>This handler processes a <see cref="GetCustomerCasesByIdentificationNumberQuery"/> and returns a <see
/// cref="GetCustomerCasesByIdentificationNumberResponse"/> containing the customer cases associated with the provided
/// identification number. If the identification number is null or whitespace, an error message is set in the
/// response.</remarks>
public class GetCustomerCasesByIdentificationNumberQueryHandler : SharedFeatures, IRequestHandler<GetCustomerCasesByIdentificationNumberQuery, GetCustomerCasesByIdentificationNumberResponse>
{
    private readonly Services.IOMCaseService _caseService;

    public GetCustomerCasesByIdentificationNumberQueryHandler
        (
            ILoggingService loggingService,
            Services.IOMCaseService caseService
        )
        : base(loggingService)
    {
        _caseService = caseService;
    }

    /// <summary>
    /// Handles the query to retrieve customer cases based on the provided identification number.
    /// </summary>
    /// <remarks>The identification number provided in the query must not be null, empty, or consist only of
    /// whitespace. If the identification number is invalid, the response will include an appropriate error
    /// message.</remarks>
    /// <param name="request">The query containing the identification number used to retrieve customer cases.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="GetCustomerCasesByIdentificationNumberResponse"/> with the list of customer cases if the identification
    /// number is valid, or an error message if the identification number is missing or invalid.</returns>
    public async Task<GetCustomerCasesByIdentificationNumberResponse> Handle(GetCustomerCasesByIdentificationNumberQuery request, CancellationToken cancellationToken)
    {
        GetCustomerCasesByIdentificationNumberResponse response = new ();

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Identification number is required.");
            return response;
        }

        OMCaseListResponse omCaseListResponse = await _caseService.GetCasesForCustomerByIdentificationNumberAsync(request.IdentificationNumber, cancellationToken);
        
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
