using MediatR;
using Microsoft.Extensions.Options;
using om.servicing.casemanagement.domain.Configuration.Options;
using OM.RequestFramework.Core.Enums;
using OM.RequestFramework.Core.Logging;
using System.Diagnostics;

namespace om.servicing.casemanagement.application.Features.Configuration;

/// <summary>
/// Provides logging behavior for MediatR request pipelines, logging details about the request and its execution time.
/// </summary>
/// <remarks>This behavior logs the start and end of the request handling process, including the execution time.
/// Additional logging is performed in non-production environments, including detailed request information.</remarks>
/// <typeparam name="TRequest">The type of the request being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
public class MediatorLoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private ILoggingService _loggingService;
    private readonly IOptions<EnvironmentOptions> _environmentOptions;

    /// <summary>
    /// Create an instance using dependency injections
    /// </summary>
    /// <param name="loggingService"></param>
    /// <param name="environmentOptions"></param>
    public MediatorLoggingBehaviour(ILoggingService loggingService, IOptions<EnvironmentOptions> environmentOptions)
    {
        _loggingService = loggingService;
        _environmentOptions = environmentOptions;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestGuid = Guid.NewGuid().ToString();
        var requestName = typeof(TRequest).Name;
        var stopWatch = Stopwatch.StartNew();
        TResponse response;

        // Request 
        _loggingService.LogInfo($"Handle {requestName} [{requestGuid}]");

        if (_environmentOptions.Value.IsNonProd)
        {
            _loggingService.LogInfoInMethod(className: "", requestName, request, $"Handling {requestName} [{requestGuid}]", LogDirection.Incoming);
        }

        try
        {
            // Action Request Handler
            response = await next();
        }
        finally
        {
            stopWatch.Stop();
            _loggingService.LogInfo($"Handled {requestName} [{requestGuid}]; Execution time: {stopWatch.ElapsedMilliseconds}ms");
        }

        // Response
        return response;
    }
}
