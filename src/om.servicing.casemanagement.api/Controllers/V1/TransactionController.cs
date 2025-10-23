using Microsoft.AspNetCore.Mvc;
using om.servicing.casemanagement.application.Features.OMTransactions.Commands;
using om.servicing.casemanagement.domain.Constants;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace om.servicing.casemanagement.api.Controllers.V1;

[ApiController]
[Route("/api/casemanagement/v{version:apiVersion}/[controller]")]
public class TransactionController : BaseController
{
    [SwaggerOperation(
        Summary = "Create a transaction.",
        Description = @"This request creates a transaction using the details provided.")]
    [HttpPost]
    [Route("create")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateOMTransactionCommandResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateOMTransaction(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem,
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XCustomerId)] string customerIdentificationNumber,
        [FromBody] CreateOMTransactionCommand command)
    {
        CreateOMTransactionCommandResponse response = await Mediator.Send(command);
        return HandleApplicationEnterpriseResponse<CreateOMTransactionCommandResponse>(response);
    }
}
