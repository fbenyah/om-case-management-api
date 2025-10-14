using MediatR;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMCases.Queries;

/// <summary>
/// Represents a query to retrieve customer cases based on their identification number and status.
/// </summary>
/// <remarks>This query is used to filter customer cases by a specific identification number and status. The
/// result of the query is returned as a <see
/// cref="GetCustomerCasesByIdentificationNumberAndStatusResponse"/>.</remarks>
public class GetCustomerCasesByIdentificationNumberAndStatusQuery : IRequest<GetCustomerCasesByIdentificationNumberAndStatusResponse>
{
    public string Status { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a list of customer cases filtered by identification number and status.
/// </summary>
/// <remarks>This response is returned by operations that retrieve customer cases based on specific identification
/// numbers and statuses. The <see cref="Data"> property contains the resulting list of cases, which will be empty if no
/// matching cases are found.</remarks>
public class GetCustomerCasesByIdentificationNumberAndStatusResponse : ApplicationBaseResponse<List<domain.Dtos.OMCaseDto>>, IResponse
{
    public GetCustomerCasesByIdentificationNumberAndStatusResponse()
    {
        Data = new List<domain.Dtos.OMCaseDto>();
    }
}

/// <summary>
/// Handles queries to retrieve customer cases based on an identification number and case status.
/// </summary>
/// <remarks>This handler processes a query to fetch customer cases that match the specified identification number
/// and status. It validates the input parameters and interacts with the case service to retrieve the data.</remarks>
public class GetCustomerCasesByIdentificationNumberAndStatusQueryHandler : SharedFeatures, IRequestHandler<GetCustomerCasesByIdentificationNumberAndStatusQuery, GetCustomerCasesByIdentificationNumberAndStatusResponse>
{
    private readonly Services.IOMCaseService _caseService;

    public GetCustomerCasesByIdentificationNumberAndStatusQueryHandler
        (
            ILoggingService loggingService,
            Services.IOMCaseService caseService
        )
        : base(loggingService)
    {
        _caseService = caseService;
    }

    public async Task<GetCustomerCasesByIdentificationNumberAndStatusResponse> Handle(GetCustomerCasesByIdentificationNumberAndStatusQuery request, CancellationToken cancellationToken)
    {
        GetCustomerCasesByIdentificationNumberAndStatusResponse response = new();

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Identification number is required.");
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            response.SetOrUpdateErrorMessage("Status of case(s) is required.");
            return response;
        }

        List<domain.Dtos.OMCaseDto> customerCases = await _caseService.GetCasesForCustomerByStatusAsync(request.IdentificationNumber, request.Status);
        response.Data = customerCases;

        return response;
    }
}
