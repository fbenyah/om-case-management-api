using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Exceptions.Client;
using om.servicing.casemanagement.domain.Mappings;
using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMInteractionUtilities
{
    /// <summary>
    /// Converts a collection of <see cref="OMInteraction"/> objects to a list of <see cref="OMInteractionDto"/>
    /// objects.
    /// </summary>
    /// <param name="omInteractions">The collection of <see cref="OMInteraction"/> objects to convert. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects. Returns an empty list if <paramref name="omInteractions"/> is
    /// null or contains no elements.</returns>
    public static List<OMInteractionDto> ReturnInteractionDtoList(IEnumerable<OMInteraction> omInteractions)
    {
        List<OMInteractionDto> interactionDtoList = new List<OMInteractionDto>();

        if (omInteractions == null || !omInteractions.Any())
        {
            return interactionDtoList;
        }

        foreach (OMInteraction omInteraction in omInteractions)
        {
            interactionDtoList.Add(EntityToDtoMapper.ToDto(omInteraction));
        }

        return interactionDtoList;
    }

    /// <summary>
    /// Converts a list of interaction DTOs to a list of interaction entities.
    /// </summary>
    /// <param name="interactionDtoList">A list of <see cref="OMInteractionDto"/> objects to be converted.  If the list is null or empty, an empty list
    /// is returned.</param>
    /// <returns>A list of <see cref="OMInteraction"/> objects converted from the provided DTOs.  Returns an empty list if
    /// <paramref name="interactionDtoList"/> is null or empty.</returns>
    public static List<OMInteraction> ReturnInteractionList(List<OMInteractionDto> interactionDtoList)
    {
        List<OMInteraction> interactionList = new List<OMInteraction>();

        if (interactionDtoList == null || !interactionDtoList.Any())
        {
            return interactionList;
        }

        foreach (OMInteractionDto omInteractionDto in interactionDtoList)
        {
            interactionList.Add(DtoToEntityMapper.ToEntity(omInteractionDto));
        }

        return interactionList;
    }

    /// <summary>
    /// Determines whether the specified interaction is eligible for creating another entity by validating the
    /// interaction data and updating the response with any errors or exceptions.
    /// </summary>
    /// <remarks>This method performs the following validations: <list type="bullet"> <item> If the retrieval
    /// of interactions is unsuccessful, the response is updated with the error messages and any custom exceptions
    /// returned by the service. </item> <item> If no interactions are found for the specified interaction ID, the
    /// response is updated with an error message indicating that no interaction was found. </item> <item> If multiple
    /// interactions are found for the specified interaction ID, the response is updated with an error message and a
    /// <see cref="ConflictException"/> is added to indicate the conflict. </item> </list></remarks>
    /// <typeparam name="TResponse">The type of the response object, which must inherit from <see cref="BaseFluentValidationError"/>.</typeparam>
    /// <param name="interactionId">The unique identifier of the interaction to validate.</param>
    /// <param name="response">The response object to update with error messages or exceptions if the interaction is not eligible.</param>
    /// <param name="interactionService">The service used to retrieve interaction data for the specified interaction ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns></returns>
    public async static Task<OMInteractionListResponse> DetermineIfInteractionIsEligibleForOtherEntityCreation<TResponse>(string interactionId, TResponse response, Services.IOMInteractionService interactionService, CancellationToken cancellationToken)
        where TResponse : BaseFluentValidationError
    {
        OMInteractionListResponse omInteractionListResponse = await interactionService.GetInteractionsForInteractionIdAsync(interactionId, null, cancellationToken);

        if (!omInteractionListResponse.Success)
        {

            response.SetOrUpdateErrorMessages(omInteractionListResponse.ErrorMessages);

            if (omInteractionListResponse.CustomExceptions != null && omInteractionListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omInteractionListResponse.CustomExceptions);
            }
        }

        if (omInteractionListResponse.Data == null || omInteractionListResponse.Data.Count == 0)
        {
            response.SetOrUpdateErrorMessage($"No interaction found for Interaction Id: {interactionId}");
            response.SetOrUpdateErrorMessages(omInteractionListResponse.ErrorMessages);

            if (omInteractionListResponse.CustomExceptions != null && omInteractionListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omInteractionListResponse.CustomExceptions);
            }
        }

        if (omInteractionListResponse.Data.Count > 1)
        {
            string errorMessage = $"Multiple interactions found for Interaction Id: {interactionId}";
            response.SetOrUpdateErrorMessage(errorMessage);
            response.SetOrUpdateCustomException(new ConflictException(errorMessage));
        }

        return omInteractionListResponse;
    }

    /// <summary>
    /// Prepare an OMInteraction entity for persistence by ensuring FK properties are set and clearing navigation properties
    /// that would cause EF Core to attempt to attach duplicate tracked entities (for example OMCase).
    /// Returns preserved values you may need after save (case id/reference).
    /// </summary>
    public static (string? PreservedCaseReferenceNumber, string? PreservedCaseId) PrepareInteractionForPersistence(OMInteraction interaction)
    {
        if (interaction == null) throw new ArgumentNullException(nameof(interaction));

        string? preservedCaseReferenceNumber = interaction.Case?.ReferenceNumber;
        string? preservedCaseId = interaction.Case?.Id;

        // Ensure FK is set (prefer the navigation value if present)
        if (!string.IsNullOrWhiteSpace(interaction.Case?.Id))
        {
            interaction.CaseId = interaction.Case.Id;
        }

        // Clear top-level Case navigation so EF won't try to attach a second OMCase instance
        interaction.Case = null;

        // If transactions exist, ensure their FKs are set and clear their navigations too.
        if (interaction.Transactions != null)
        {
            foreach (var tx in interaction.Transactions)
            {
                if (!string.IsNullOrWhiteSpace(tx.Case?.Id))
                {
                    tx.CaseId = tx.Case.Id;
                }

                if (!string.IsNullOrWhiteSpace(tx.Interaction?.Id))
                {
                    tx.InteractionId = tx.Interaction.Id;
                }

                if (tx.TransactionType != null && !string.IsNullOrWhiteSpace(tx.TransactionType.Id))
                {
                    tx.TransactionTypeId = tx.TransactionType.Id;
                }

                tx.Case = null;
                tx.Interaction = null;
                tx.TransactionType = null;
            }
        }

        return (preservedCaseReferenceNumber, preservedCaseId);
    }
}
