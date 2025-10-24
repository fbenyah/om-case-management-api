using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Extensions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMCases.Commands;

public class CreateOMCaseCommand : IRequest<CreateOMCaseCommandResponse>
{
    public CaseChannel SourceChannel { get; set; } = CaseChannel.Unknown;
    public string IdentificationNumber { get; set; } = string.Empty;
}

public class CreateOMCaseCommandResponse : ApplicationBaseResponse<BasicCaseCreateResponse>
{
    public CreateOMCaseCommandResponse()
    {
        Data = new BasicCaseCreateResponse();
    }
}

public class CreateOMCaseCommandHandler : SharedFeatures, IRequestHandler<CreateOMCaseCommand, CreateOMCaseCommandResponse>
{
    private readonly Services.IOMCaseService _caseService;

    public CreateOMCaseCommandHandler
        (
            ILoggingService loggingService,
            Services.IOMCaseService caseService
        )
        : base(loggingService)
    {
        _caseService = caseService;
    }

    public async Task<CreateOMCaseCommandResponse> Handle(CreateOMCaseCommand command, CancellationToken cancellationToken)
    {
        var response = new CreateOMCaseCommandResponse();

        //TO:DO add validations for command

        OMCaseDto omCaseDto = new()
        {
            Channel = command.SourceChannel.GetDescription(),
            IdentificationNumber = command.IdentificationNumber,
            Status = CaseStatus.Initiated.GetDescription()
        };

        OMCaseCreateResponse createCaseserviceResponse = await _caseService.CreateCaseAsync(omCaseDto, cancellationToken);
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
