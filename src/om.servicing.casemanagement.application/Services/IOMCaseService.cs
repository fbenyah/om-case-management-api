using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMCaseService
{
    Task<List<OMCaseDto>> GetCasesForCustomer(string identityNumber);
    Task<List<OMCaseDto>> GetCasesForCustomerByStatusAsync(string identityNumber, string status);
}
