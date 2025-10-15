using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Extensions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMCases.Commands;

public class CreateShellCaseCommand : IRequest<CreateShellCaseCommandResponse>
{
    public CaseChannel SourceChannel { get; set; } = CaseChannel.Unknown;
}

public class CreateShellCaseCommandResponse : ApplicationBaseResponse<BasicCaseCreateResponse>
{
    public CreateShellCaseCommandResponse()
    {
        Data = new BasicCaseCreateResponse();
    }
}

public class CreateShellCaseCommandHandler : SharedFeatures, IRequestHandler<CreateShellCaseCommand, CreateShellCaseCommandResponse>
{
    private readonly Services.IOMCaseService _caseService;

    public CreateShellCaseCommandHandler
        (
            ILoggingService loggingService,
            Services.IOMCaseService caseService
        )
        : base(loggingService)
    {
        _caseService = caseService;
    }

    public async Task<CreateShellCaseCommandResponse> Handle(CreateShellCaseCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateShellCaseCommandResponse();

        OMCaseDto omCaseDto = new()
        {
            Channel = request.SourceChannel.GetDescription()
        };

        omCaseDto.Status = CaseStatus.Initiated.GetDescription();

        OMCaseCreateResponse createCaseserviceResponse = await _caseService.CreateCaseAsync(new OMCaseDto(), cancellationToken);
        if (!createCaseserviceResponse.Success)
        {
            response.SetOrUpdateErrorMessage("Failed to create shell case.");

            if (createCaseserviceResponse.ErrorMessages != null && createCaseserviceResponse.ErrorMessages.Any())
            {
                response.SetOrUpdateErrorMessages(createCaseserviceResponse.ErrorMessages);
            }

            if (createCaseserviceResponse.CustomExceptions != null && createCaseserviceResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(createCaseserviceResponse.CustomExceptions);
            }

            return response;
        }

        response.Data = createCaseserviceResponse.Data;
        return response;
    }
}
