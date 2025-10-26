using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Extensions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMInteractions.Commands;

public class CreateOMInteractionCommand : IRequest<CreateOMInteractionCommandResponse>
{
    public string CaseId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsPrimaryInteraction { get; set; } = true;
    public string PreviousInteractionId { get; set; } = string.Empty;
}

public class CreateOMInteractionCommandResponse : ApplicationBaseResponse<BasicInteractionCreateResponse>
{
    public CreateOMInteractionCommandResponse()
    {
        Data = new BasicInteractionCreateResponse();
    }
}

public class CreateOMInteractionCommandHandler : SharedFeatures, IRequestHandler<CreateOMInteractionCommand, CreateOMInteractionCommandResponse>
{
    private readonly Services.IOMCaseService _caseService;
    private readonly Services.IOMInteractionService _interactionService;

    public CreateOMInteractionCommandHandler
        (
            ILoggingService loggingService,
            Services.IOMCaseService caseService,
            Services.IOMInteractionService interactionService
        )
        : base(loggingService)
    {
        _caseService = caseService;
        _interactionService = interactionService;
    }

    public async Task<CreateOMInteractionCommandResponse> Handle(CreateOMInteractionCommand command, CancellationToken cancellationToken)
    {
        var response = new CreateOMInteractionCommandResponse();

        //TO:DO add validations for command

        OMCaseListResponse omCaseListResponse = await OMCaseUtilities.DetermineIfCaseIsEligibleForOtherEntityCreation<CreateOMInteractionCommandResponse>(command.CaseId, response, _caseService, cancellationToken);

        if (!response.Success || !omCaseListResponse.Success)
        {
            return response;
        }

        // if not primary interaction, validate previous interaction exists
        if (!command.IsPrimaryInteraction)
        {
            OMInteractionExistsResponse omInteractionExistsResponse = await _interactionService.InteractionOnCaseExistsWithIdAsync(command.PreviousInteractionId, command.CaseId, cancellationToken);
            if (!omInteractionExistsResponse.Success)
            {
                response.SetOrUpdateErrorMessages(omInteractionExistsResponse.ErrorMessages);

                if (omInteractionExistsResponse.CustomExceptions != null && omInteractionExistsResponse.CustomExceptions.Any())
                {
                    response.SetOrUpdateCustomExceptions(omInteractionExistsResponse.CustomExceptions);
                }

                return response;
            }
        }

        // TO:DO ensure the previous interaction belongs to the same case
        // if it does belong to the same case, close the loop by updating the previous interaction status to Closed
        // This can be done in the interaction service layer
        // For now, we will just create the new interaction

        OMInteractionDto omInteractionDto = new()
        {
            Notes = command.Notes,
            IsPrimaryInteraction = command.IsPrimaryInteraction,
            PreviousInteractionId = command.PreviousInteractionId,            
            Case = omCaseListResponse.Data.First(),
            Status = InteractionStatus.Initiated.GetDescription()
        };

        OMInteractionCreateResponse omInteractionCreateResponse = await _interactionService.CreateInteractionAsync(omInteractionDto, cancellationToken);

        if (!omInteractionCreateResponse.Success)
        {
            response.SetOrUpdateErrorMessages(omInteractionCreateResponse.ErrorMessages);

            if (omInteractionCreateResponse.CustomExceptions != null && omInteractionCreateResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omInteractionCreateResponse.CustomExceptions);
            }

            return response;
        }

        response.Data = omInteractionCreateResponse.Data;
        return response;
    }
}