using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace om.servicing.casemanagement.api.Controllers.V1;

[ApiController]
[Route("/api/casemanagement/v{version:apiVersion}/[controller]")]
public class HealthCheckController : BaseController
{
    [SwaggerOperation(
        Summary = "Health check endpoint.",
        Description = @"This request is to be used for the health check for this api.")]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Get()
    {
        string message = $"CASE MANAGEMENT API is running - {DateTime.Now.ToString()}";

        LoggingService.LogInfo(message);
        return Ok(message);
    }
}
