using MediatR;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberAndStatusQuery : IRequest<GetCustomerCasesByIdentificationNumberAndStatusResponse>
{
    public string Status { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
}

public class GetCustomerCasesByIdentificationNumberAndStatusResponse : ApplicationBaseResponse<List<domain.Dtos.OMCaseDto>>, IResponse
{
    public GetCustomerCasesByIdentificationNumberAndStatusResponse()
    {
        Data = new List<domain.Dtos.OMCaseDto>();
    }
}

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
