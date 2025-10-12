using Microsoft.AspNetCore.Mvc;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.domain.Constants;
using om.servicing.casemanagement.domain.Responses.Shared;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace om.servicing.casemanagement.api.Controllers.V1;

[ApiController]
[Route("/api/casemanagement/v{version:apiVersion}/[controller]")]
public class CaseContoller : BaseController
{
    [SwaggerOperation(
        Summary = "Customer cases by indentification number.",
        Description = @"This request gets all customer cases given the identification number linked to that case.")]
    [HttpGet]
    [Route("by/identification/{identificationNumber}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetCustomerCasesByIdentification(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] string sourceSystem, 
        string identificationNumber)
    {
        GetCustomerCasesByIdentificationNumberResponse response = await Mediator.Send(new GetCustomerCasesByIdentificationNumberQuery
        {
            IdentificationNumber = identificationNumber
        });

        return HandleApplicationEnterpriseResponse<GetCustomerCasesByIdentificationNumberResponse>(response);
    }
}
