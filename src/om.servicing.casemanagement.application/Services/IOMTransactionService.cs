using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMTransactionService
{
    Task<List<OMTransactionDto>> GetTransactionsForCaseByCaseIdAsync(string caseId);

    Task<List<OMTransactionDto>> GetTransactionsForCaseByCustomerIdentificationAsync(string customerIdentificationNumber);
}
