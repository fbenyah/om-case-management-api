using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Exceptions.Client;
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
    public CaseChannel SourceSystem { get; set; } = CaseChannel.Unknown;
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

    public async Task<CreateOMInteractionCommandResponse> Handle(CreateOMInteractionCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateOMInteractionCommandResponse();

        OMCaseListResponse omCaseListResponse = await _caseService.GetCasesForCustomerByCaseId(request.CaseId, cancellationToken);
        
        if (!omCaseListResponse.Success)
        {

            response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);

            if (omCaseListResponse.CustomExceptions != null && omCaseListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }

            return response;
        }

        if (omCaseListResponse.Data == null || omCaseListResponse.Data.Count == 0)
        {
            response.SetOrUpdateErrorMessage($"No case found for CaseId: {request.CaseId}");
            response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);

            if (omCaseListResponse.CustomExceptions != null && omCaseListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }

            return response;
        }

        if (omCaseListResponse.Data.Count > 1)
        {
            string errorMessage = $"Multiple cases found for CaseId: {request.CaseId}";
            response.SetOrUpdateErrorMessage(errorMessage);
            response.SetOrUpdateCustomException(new ConflictException(errorMessage));
            return response;
        }

        OMInteractionDto omInteractionDto = new()
        {
            Notes = request.Notes,
            IsPrimaryInteraction = request.IsPrimaryInteraction,
            PreviousInteractionId = request.PreviousInteractionId,            
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