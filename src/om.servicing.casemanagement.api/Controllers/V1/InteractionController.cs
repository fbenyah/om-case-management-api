using Microsoft.AspNetCore.Mvc;
using om.servicing.casemanagement.application.Features.OMInteractions.Commands;
using om.servicing.casemanagement.domain.Constants;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace om.servicing.casemanagement.api.Controllers.V1;

[ApiController]
[Route("/api/casemanagement/v{version:apiVersion}/[controller]")]
public class InteractionController : BaseController
{
    [SwaggerOperation(
        Summary = "Create an interaction.",
        Description = @"This request creates an interaction using the details provided.")]
    [HttpPost]
    [Route("create")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateOMInteractionCommandResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateOMInteraction(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem,
        [FromBody] CreateOMInteractionCommand command)
    {
        CreateOMInteractionCommandResponse response = await Mediator.Send(command);
        return HandleApplicationEnterpriseResponse<CreateOMInteractionCommandResponse>(response);
    }
}
