using MediatR;
using Microsoft.AspNetCore.Mvc;
using om.servicing.casemanagement.domain.Exceptions.Client;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.api.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    private IMediator _mediator;
    private ILoggingService _loggingService;

    protected IMediator Mediator => LazyRequireServiceMediator();
    protected ILoggingService LoggingService => LazyRequireServiceLogging();

    private IMediator LazyRequireServiceMediator()
    {
        if (_mediator != null) return _mediator;

        _mediator = HttpContext.RequestServices.GetService<IMediator>();

        if (_mediator == null)
            throw new Exception("Unable to initiate mediator services");

        return _mediator;
    }

    private ILoggingService LazyRequireServiceLogging()
    {
        if (_loggingService != null) return _loggingService;

        _loggingService = HttpContext.RequestServices.GetService<ILoggingService>();

        if (_loggingService == null)
            throw new Exception("Unable to initiate logging services");

        return _loggingService;
    }

    protected IActionResult HandleApplicationEnterpriseResponse<T>(T response) where T : BaseFluentValidationError
    {
        if (!response.Success)
        {
            if (response.CustomExceptions is not null)
            {
                if (response.CustomExceptions.Any(x => x is ConflictException))
                {
                    return Conflict(response);
                }

                if (response.CustomExceptions.Any(x => x is NotFoundException))
                {
                    return NoContent();
                }

                if (response.CustomExceptions.Any(x => x is TooManyRequestsException))
                {
                    return StatusCode(429, response);
                }
            }

            return BadRequest(response);
        }

        return Ok(response);
    }
}
