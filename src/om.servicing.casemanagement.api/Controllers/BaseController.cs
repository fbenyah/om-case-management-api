using MediatR;
using Microsoft.AspNetCore.Mvc;
using om.servicing.casemanagement.domain.Exceptions.Client;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;
    private ILoggingService _loggingService;

    protected IMediator Mediator => LazyRequireServiceMediator();
    protected ILoggingService LoggingService => LazyRequireServiceLogging();

    /// <summary>
    /// Lazily retrieves the <see cref="IMediator"/> service from the current HTTP context's service provider.
    /// </summary>
    /// <remarks>This method initializes the <see cref="IMediator"/> instance if it has not already been set. 
    /// If the service cannot be resolved, an exception is thrown.</remarks>
    /// <returns>The <see cref="IMediator"/> instance retrieved from the service provider.</returns>
    /// <exception cref="Exception">Thrown if the <see cref="IMediator"/> service cannot be resolved from the HTTP context's service provider.</exception>
    private IMediator LazyRequireServiceMediator()
    {
        if (_mediator != null) return _mediator;

        _mediator = HttpContext.RequestServices.GetService<IMediator>();

        if (_mediator == null)
            throw new Exception("Unable to initiate mediator services");

        return _mediator;
    }

    /// <summary>
    /// Retrieves the logging service instance, initializing it if necessary.
    /// </summary>
    /// <remarks>This method attempts to resolve the <see cref="ILoggingService"/> from the current HTTP
    /// context's service provider. If the service is not available, an exception is thrown.</remarks>
    /// <returns>The initialized <see cref="ILoggingService"/> instance.</returns>
    /// <exception cref="Exception">Thrown if the logging service cannot be resolved from the service provider.</exception>
    private ILoggingService LazyRequireServiceLogging()
    {
        if (_loggingService != null) return _loggingService;

        _loggingService = HttpContext.RequestServices.GetService<ILoggingService>();

        if (_loggingService == null)
            throw new Exception("Unable to initiate logging services");

        return _loggingService;
    }

    /// <summary>
    /// Handles the response for an application enterprise operation and returns an appropriate HTTP status code.
    /// </summary>
    /// <remarks>This method evaluates the <paramref name="response"/> to determine the appropriate HTTP
    /// status code based on the success flag  and the presence of specific exception types in the
    /// <c>CustomExceptions</c> collection. If no matching exception is found,  a <see
    /// cref="StatusCodes.Status400BadRequest"/> response is returned for unsuccessful operations.</remarks>
    /// <typeparam name="T">The type of the response, which must inherit from <see cref="BaseFluentValidationError"/>.</typeparam>
    /// <param name="response">The response object containing the operation result, success status, and any associated exceptions.</param>
    /// <returns>An <see cref="IActionResult"/> representing the HTTP response: <list type="bullet"> <item><description><see
    /// cref="StatusCodes.Status200OK"/> if the operation was successful.</description></item> <item><description><see
    /// cref="StatusCodes.Status400BadRequest"/> if the operation failed without specific
    /// exceptions.</description></item> <item><description><see cref="StatusCodes.Status409Conflict"/> if a <see
    /// cref="ConflictException"/> is present in the response.</description></item> <item><description><see
    /// cref="StatusCodes.Status204NoContent"/> if a <see cref="NotFoundException"/> is present in the
    /// response.</description></item> <item><description><see cref="StatusCodes.Status429TooManyRequests"/> if a <see
    /// cref="TooManyRequestsException"/> is present in the response.</description></item> </list></returns>
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
