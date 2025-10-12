using MediatR;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.application.Features.OMCases.Queries;

public class GetCustomerCasesByIdentificationNumberQuery : IRequest<GetCustomerCasesByIdentificationNumberResponse>
{
    public string IdentificationNumber { get; set; } = string.Empty;
}

public class GetCustomerCasesByIdentificationNumberResponse : ApplicationBaseResponse<List<domain.Dtos.OMCaseDto>>, IResponse
{
    public GetCustomerCasesByIdentificationNumberResponse()
    {
        Data = new List<domain.Dtos.OMCaseDto>();
    }
}

public class GetCustomerCasesByIdentificationNumberQueryHandler : IRequestHandler<GetCustomerCasesByIdentificationNumberQuery, GetCustomerCasesByIdentificationNumberResponse>
{
    private readonly Services.IOMCaseService _caseService;
    public GetCustomerCasesByIdentificationNumberQueryHandler(Services.IOMCaseService caseService)
    {
        _caseService = caseService;
    }


    public async Task<GetCustomerCasesByIdentificationNumberResponse> Handle(GetCustomerCasesByIdentificationNumberQuery request, CancellationToken cancellationToken)
    {
        GetCustomerCasesByIdentificationNumberResponse response = new ();

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            response.SetOrUpdateErrorMessage("Identification number is required.");
            return response;
        }

        List<domain.Dtos.OMCaseDto> customerCases = await _caseService.GetCasesForCustomer(request.IdentificationNumber);
        response.Data = customerCases;

        return response;
    }
}
