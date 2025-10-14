using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.application.Utilities;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.application.Services;

public class OMTransactionService : IOMTransactionService
{
    private readonly IOMCaseService _omCaseService;
    private readonly IOMInteractionService _omInteractionService;
    private readonly IGenericRepository<OMTransaction> _transactionRepository;

    public OMTransactionService(
        IOMCaseService oMCaseService,
        IOMInteractionService omInteractionService,
        IGenericRepository<OMTransaction> transactionRepository
        )
    {
        _omCaseService = oMCaseService;
        _omInteractionService = omInteractionService;
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Retrieves a list of transactions associated with the specified case.
    /// </summary>
    /// <param name="caseId">The unique identifier of the case for which transactions are to be retrieved. Cannot be null, empty, or
    /// whitespace.</param>
    /// <returns>A list of <see cref="OMTransactionDto"/> objects representing the transactions for the specified case.  Returns
    /// an empty list if the <paramref name="caseId"/> is null, empty, or whitespace, or if no transactions are found.</returns>
    public async Task<List<OMTransactionDto>> GetTransactionsForCaseByCaseIdAsync(string caseId)
    {
        if (string.IsNullOrWhiteSpace(caseId))
        {
            return new List<OMTransactionDto>();
        }

        IEnumerable<OMTransaction> omTransactions = await _transactionRepository.FindAsync(t => t.CaseId == caseId);

        return OMTransactionUtilities.ReturnTransactionDtoList(omTransactions);
    }

    /// <summary>
    /// Retrieves a list of transactions associated with a customer's cases based on the provided customer
    /// identification number.
    /// </summary>
    /// <remarks>This method retrieves all cases associated with the specified customer and then fetches the
    /// transactions for each case. If no cases are found for the customer, or if the customer identification number is
    /// invalid, an empty list is returned.</remarks>
    /// <param name="customerIdentificationNumber">The identification number of the customer whose case transactions are to be retrieved.  Cannot be null, empty,
    /// or consist only of whitespace.</param>
    /// <returns>A list of <see cref="OMTransactionDto"/> objects representing the transactions associated with the customer's
    /// cases. Returns an empty list if the customer has no cases or if the provided identification number is invalid.</returns>
    public async Task<List<OMTransactionDto>> GetTransactionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber)
    {
        if (string.IsNullOrWhiteSpace(customerIdentificationNumber))
        {
            return new List<OMTransactionDto>();
        }

        OMCaseListResponse omCaseListResponse = await _omCaseService.GetCasesForCustomerByIdentificationNumberAsync(customerIdentificationNumber);

        if (!omCaseListResponse.Success)
        {
            return new List<OMTransactionDto>();
        }

        if (omCaseListResponse.Data == null || !omCaseListResponse.Data.Any())
        {
            return new List<OMTransactionDto>();
        }

        List<OMTransactionDto> allTransactionsDto = new();

        foreach (OMCaseDto omCaseDto in omCaseListResponse.Data)
        {
            List<OMTransactionDto> transactionsForCase = await GetTransactionsForCaseByCaseIdAsync(omCaseDto.Id);
            if (transactionsForCase != null && transactionsForCase.Any())
            {
                allTransactionsDto.AddRange(transactionsForCase);
            }
        }

        return allTransactionsDto;
    }

    /// <summary>
    /// Retrieves a list of transactions associated with a specific interaction for a given customer.
    /// </summary>
    /// <remarks>This method first retrieves all interactions for the specified customer and filters them by
    /// the provided interaction ID. If transactions are not already loaded in the interaction data, they are fetched
    /// using the associated case ID.</remarks>
    /// <param name="customerIdentificationNumber">The unique identification number of the customer. This value cannot be null, empty, or consist only of
    /// whitespace.</param>
    /// <param name="interactionId">The unique identifier of the interaction. This value cannot be null, empty, or consist only of whitespace.</param>
    /// <returns>A list of <see cref="OMTransactionDto"/> objects representing the transactions associated with the specified
    /// interaction. Returns an empty list if no transactions are found or if the input parameters are invalid.</returns>
    public async Task<List<OMTransactionDto>> GetTransactionsForInteractionByCustomerIdentificationAsync(string customerIdentificationNumber, string interactionId)
    {
        if (string.IsNullOrWhiteSpace(customerIdentificationNumber) 
            || string.IsNullOrWhiteSpace(interactionId))
        {
            return new List<OMTransactionDto>();
        }

        List<OMInteractionDto>? omInteractionsDto = await _omInteractionService.GetInteractionsForCaseByCustomerIdentificationAsync(customerIdentificationNumber);
        omInteractionsDto = omInteractionsDto?.Where(i => i.Id == interactionId)?.ToList();

        if (omInteractionsDto == null || !omInteractionsDto.Any())
        {
            return new List<OMTransactionDto>();
        }

        List<OMTransactionDto> allTransactionsDto = new();

        foreach (OMInteractionDto omInteractionDto in omInteractionsDto)
        {
            if (omInteractionDto.Transactions != null && omInteractionDto.Transactions.Any())
            {
                allTransactionsDto.AddRange(omInteractionDto.Transactions);
                continue;
            }

            // If transactions are not already loaded in the interaction DTO, fetch them by case ID
            List<OMTransactionDto> transactionsForInteraction = await GetTransactionsForCaseByCaseIdAsync(omInteractionDto.Case?.Id);
            if (transactionsForInteraction != null && transactionsForInteraction.Any())
            {
                allTransactionsDto.AddRange(transactionsForInteraction);
            }
        }

        return allTransactionsDto;
    }
}
