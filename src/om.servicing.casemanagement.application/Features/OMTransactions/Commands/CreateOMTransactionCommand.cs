using MediatR;
using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Extensions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.OMTransactions.Commands;

public class CreateOMTransactionCommand : IRequest<CreateOMTransactionCommandResponse>
{
    public string CaseId { get; set; } = string.Empty;
    public string InteractionId { get; set; } = string.Empty;
    public bool IsImmediate { get; set; } = true;
    public bool IsFulfilledExternally { get; set; } = false;
    public string ReceivedDetails { get; set; } = string.Empty;
    public string ProcessedDetails { get; set; } = string.Empty;
}

public class CreateOMTransactionCommandResponse : ApplicationBaseResponse<BasicTransactionCreateResponse>
{
    public CreateOMTransactionCommandResponse()
    {
        Data = new BasicTransactionCreateResponse();
    }
}

public class CreateOMTransactionCommandHandler : SharedFeatures, IRequestHandler<CreateOMTransactionCommand, CreateOMTransactionCommandResponse>
{
    private readonly Services.IOMCaseService _caseService;
    private readonly Services.IOMInteractionService _interactionService;
    private readonly Services.IOMTransactionService _transactionService;

    public CreateOMTransactionCommandHandler(
        ILoggingService loggingService,
        Services.IOMCaseService caseService,
        Services.IOMInteractionService interactionService,
        Services.IOMTransactionService transactionService
        ) : base(loggingService)
    {
        _caseService = caseService;
        _interactionService = interactionService;
        _transactionService = transactionService;
    }

    public async Task<CreateOMTransactionCommandResponse> Handle(CreateOMTransactionCommand command, CancellationToken cancellationToken)
    {
        var response = new CreateOMTransactionCommandResponse();

        //TO:DO add validations for command

        OMCaseListResponse omCaseListResponse = new();
        await OMCaseUtilities.DetermineIfCaseIsEligibleForOtherEntityCreation<CreateOMTransactionCommandResponse>(command.CaseId, omCaseListResponse, response, _caseService, cancellationToken);
        
        if (!response.Success)
        {
            return response;
        }

        OMInteractionListResponse? oMInteractionListResponse = null;
        if (!string.IsNullOrWhiteSpace(command.InteractionId))
        {
            oMInteractionListResponse = new();
            await OMInteractionUtilities.DetermineIfInteractionIsEligibleForOtherEntityCreation<CreateOMTransactionCommandResponse>(command.InteractionId, oMInteractionListResponse, response, _interactionService, cancellationToken);

            if (!response.Success)
            {
                return response;
            }
        }        

        OMTransactionDto omTransactionDto = new()
        {
            Case = omCaseListResponse.Data.First(),
            IsFulfilledExternally = command.IsFulfilledExternally,
            IsImmediate = command.IsImmediate,
            ReceivedDetails = command.ReceivedDetails,
            Status = TransactionStatus.Received.GetDescription(),
        };

        if (oMInteractionListResponse != null && oMInteractionListResponse.Data.Any())
        {
            omTransactionDto.Interaction = oMInteractionListResponse.Data.First();
        }

        OMTransactionCreateResponse omTransactionCreateResponse = await _transactionService.CreateTransactionAsync(omTransactionDto, cancellationToken);

        if (!omTransactionCreateResponse.Success)
        {
            response.SetOrUpdateErrorMessages(omTransactionCreateResponse.ErrorMessages);

            if (omTransactionCreateResponse.CustomExceptions != null && omTransactionCreateResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omTransactionCreateResponse.CustomExceptions);
            }

            return response;
        }

        response.Data = omTransactionCreateResponse.Data;

        return response;
    }
}