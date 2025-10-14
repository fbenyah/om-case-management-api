using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.application.Services;

public class OMInteractionService : IOMInteractionService
{
    private readonly IOMCaseService _omCaseService;
    private readonly IGenericRepository<OMInteraction> _interactionRepository;

    public OMInteractionService(
        IOMCaseService oMCaseService,
        IGenericRepository<OMInteraction> interactionRepository
        )
    {
        _omCaseService = oMCaseService;
        _interactionRepository = interactionRepository;
    }

    /// <summary>
    /// Retrieves a list of interactions associated with the specified case ID.
    /// </summary>
    /// <remarks>This method queries the interaction repository to find interactions associated with the given
    /// case ID and converts them into a list of data transfer objects (<see cref="OMInteractionDto"/>).</remarks>
    /// <param name="caseId">The unique identifier of the case for which interactions are to be retrieved. Cannot be null, empty, or
    /// whitespace.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects representing the interactions for the specified case. Returns
    /// an empty list if the <paramref name="caseId"/> is null, empty, or whitespace, or if no interactions are found.</returns>
    public async Task<List<OMInteractionDto>> GetInteractionsForCaseByCaseIdAsync(string caseId)
    {
        if (string.IsNullOrWhiteSpace(caseId))
        {
            return new List<OMInteractionDto>();
        }

        IEnumerable<OMInteraction> omInteractions = await _interactionRepository.FindAsync(t => t.CaseId == caseId);

        return OMInteractionUtilities.ReturnInteractionDtoList(omInteractions);
    }

    /// <summary>
    /// Retrieves a list of interactions associated with the cases for a given customer identification number.
    /// </summary>
    /// <remarks>This method aggregates interactions from all cases associated with the specified customer. If
    /// no cases are found for the customer, or if the customer identification number is invalid, the method returns an
    /// empty list.</remarks>
    /// <param name="customerIdentificationNumber">The unique identification number of the customer. This value cannot be null, empty, or consist only of
    /// whitespace.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects representing the interactions associated with the customer's
    /// cases. Returns an empty list if the customer has no cases or if the provided identification number is invalid.</returns>
    public async Task<List<OMInteractionDto>> GetInteractionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber)
    {
        if (string.IsNullOrWhiteSpace(customerIdentificationNumber))
        {
            return new List<OMInteractionDto>();
        }

        List<OMCaseDto> omCasesDto = await _omCaseService.GetCasesForCustomer(customerIdentificationNumber);

        if (omCasesDto == null || !omCasesDto.Any())
        {
            return new List<OMInteractionDto>();
        }

        List<OMInteractionDto> allInteractionsDto = new();

        foreach (OMCaseDto omCaseDto in omCasesDto)
        {
            List<OMInteractionDto> interactionsForCase = await GetInteractionsForCaseByCaseIdAsync(omCaseDto.Id);
            if (interactionsForCase != null && interactionsForCase.Any())
            {
                allInteractionsDto.AddRange(interactionsForCase);
            }
        }

        return allInteractionsDto;
    }
}
