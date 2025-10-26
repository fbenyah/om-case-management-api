using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Exceptions.Client;
using om.servicing.casemanagement.domain.Mappings;
using om.servicing.casemanagement.domain.Responses.Shared;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMCaseUtilities
{
    /// <summary>
    /// Converts a collection of <see cref="OMCase"/> objects to a list of <see cref="OMCaseDto"/> objects.
    /// </summary>
    /// <param name="omCases">The collection of <see cref="OMCase"/> objects to convert. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMCaseDto"/> objects representing the converted cases.  Returns an empty list if <paramref
    /// name="omCases"/> is null or contains no elements.</returns>
    public static List<OMCaseDto> ReturnCaseDtoList(IEnumerable<OMCase> omCases)
    {
        List<OMCaseDto> caseDtoList = new List<OMCaseDto>();

        if (omCases == null || !omCases.Any())
        {
            return caseDtoList;
        }

        foreach (OMCase omCase in omCases)
        {
            caseDtoList.Add(EntityToDtoMapper.ToDto(omCase));
        }

        return caseDtoList;
    }

    /// <summary>
    /// Converts a list of data transfer objects (DTOs) to a list of entity objects.
    /// </summary>
    /// <remarks>This method iterates through the provided list of DTOs and maps each item to its
    /// corresponding entity object  using the <see cref="DtoToEntityMapper.ToEntity"/> method.</remarks>
    /// <param name="caseDtoList">The list of <see cref="OMCaseDto"/> objects to be converted. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMCase"/> objects converted from the provided DTOs.  Returns an empty list if <paramref
    /// name="caseDtoList"/> is null or empty.</returns>
    public static List<OMCase> ReturnCaseList(List<OMCaseDto> caseDtoList)
    {
        List<OMCase> caseList = new List<OMCase>();

        if (caseDtoList == null || !caseDtoList.Any())
        {
            return caseList;
        }

        foreach (OMCaseDto omCaseDto in caseDtoList)
        {
            caseList.Add(DtoToEntityMapper.ToEntity(omCaseDto));
        }

        return caseList;
    }

    /// <summary>
    /// Determines whether a case is eligible for creating another entity based on the provided case ID and response
    /// data.
    /// </summary>
    /// <remarks>This method performs the following checks to determine eligibility: <list type="bullet">
    /// <item>If the case retrieval operation fails, the response object is updated with the error messages and custom
    /// exceptions.</item> <item>If no cases are found for the given case ID, an error message is added to the response
    /// object.</item> <item>If multiple cases are found for the given case ID, a conflict error message and exception
    /// are added to the response object.</item> </list> The method updates the provided <paramref name="response"/>
    /// object with relevant error messages or exceptions based on the outcome of the checks.</remarks>
    /// <typeparam name="TResponse">The type of the response object, which must inherit from <see cref="BaseFluentValidationError"/>.</typeparam>
    /// <param name="caseId">The unique identifier of the case to evaluate. Cannot be null or empty.</param>
    /// retrieval operation.</param>
    /// <param name="response">The response object to update with error messages or exceptions if the case is not eligible for further entity
    /// creation.</param>
    /// <param name="caseService">The service used to retrieve case information based on the provided case ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. This allows the operation to be canceled if needed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async static Task<OMCaseListResponse> DetermineIfCaseIsEligibleForOtherEntityCreation<TResponse>(string caseId, TResponse response, Services.IOMCaseService caseService, CancellationToken cancellationToken)
        where TResponse : BaseFluentValidationError
    {
        OMCaseListResponse omCaseListResponse = await caseService.GetCasesForCustomerByCaseId(caseId, cancellationToken);

        if (!omCaseListResponse.Success)
        {

            response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);

            if (omCaseListResponse.CustomExceptions != null && omCaseListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }
        }

        if (omCaseListResponse.Data == null || omCaseListResponse.Data.Count == 0)
        {
            response.SetOrUpdateErrorMessage($"No case found for CaseId: {caseId}");
            response.SetOrUpdateErrorMessages(omCaseListResponse.ErrorMessages);

            if (omCaseListResponse.CustomExceptions != null && omCaseListResponse.CustomExceptions.Any())
            {
                response.SetOrUpdateCustomExceptions(omCaseListResponse.CustomExceptions);
            }
        }

        if (omCaseListResponse.Data.Count > 1)
        {
            string errorMessage = $"Multiple cases found for CaseId: {caseId}";
            response.SetOrUpdateErrorMessage(errorMessage);
            response.SetOrUpdateCustomException(new ConflictException(errorMessage));
        }

        return omCaseListResponse;
    }

    /// <summary>
    /// Prepare an OMCase entity for persistence by ensuring nested FK properties are set and clearing navigation properties
    /// that would cause EF Core to attempt to attach duplicate tracked entities (for example when child interactions or transactions
    /// contain their own navigation references). Returns preserved values you may need after save (case id/reference).
    /// </summary>
    public static (string? PreservedCaseReferenceNumber, string? PreservedCaseId) PrepareCaseForPersistence(OMCase omCase)
    {
        if (omCase == null) throw new ArgumentNullException(nameof(omCase));

        string? preservedCaseReferenceNumber = omCase.ReferenceNumber;
        string? preservedCaseId = omCase.Id;

        // If interactions exist, ensure their FK back-reference to the case is set (prefer the parent's Id if present),
        // clear their Case navigation and ensure nested transactions are prepared similarly.
        if (omCase.Interactions != null)
        {
            foreach (var interaction in omCase.Interactions)
            {
                if (!string.IsNullOrWhiteSpace(omCase.Id))
                {
                    interaction.CaseId = omCase.Id;
                }
                else if (!string.IsNullOrWhiteSpace(interaction.Case?.Id))
                {
                    interaction.CaseId = interaction.Case.Id;
                }

                // For each transaction on the interaction, set FKs from navigation if present and clear navigations
                if (interaction.Transactions != null)
                {
                    foreach (var tx in interaction.Transactions)
                    {
                        if (!string.IsNullOrWhiteSpace(omCase.Id))
                        {
                            tx.CaseId = omCase.Id;
                        }
                        else if (!string.IsNullOrWhiteSpace(tx.Case?.Id))
                        {
                            tx.CaseId = tx.Case.Id;
                        }

                        if (!string.IsNullOrWhiteSpace(interaction.Id))
                        {
                            tx.InteractionId = interaction.Id;
                        }
                        else if (!string.IsNullOrWhiteSpace(tx.Interaction?.Id))
                        {
                            tx.InteractionId = tx.Interaction.Id;
                        }

                        if (tx.TransactionType != null && !string.IsNullOrWhiteSpace(tx.TransactionType.Id))
                        {
                            tx.TransactionTypeId = tx.TransactionType.Id;
                        }

                        // Clear child navigations so EF won't try to attach duplicates
                        tx.Case = null;
                        tx.Interaction = null;
                        tx.TransactionType = null;
                    }
                }

                // Clear back-reference to case on interaction to avoid EF duplicate tracking
                interaction.Case = null;
            }
        }

        return (preservedCaseReferenceNumber, preservedCaseId);
    }
}
