using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMInteractionService
{
    Task<List<OMInteractionDto>> GetInteractionsForCaseByCaseIdAsync(string caseId);

    Task<List<OMInteractionDto>> GetInteractionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber);
}
