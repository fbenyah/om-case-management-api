using MediatR;
using MediatR.Pipeline;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features.Configuration;

/// <summary>
/// Handles exceptions that occur during the processing of a request by logging the exception details.
/// </summary>
/// <remarks>This handler logs the exception details, including any inner exceptions, using the provided logging
/// service. It does not modify the state of the request or response.</remarks>
/// <typeparam name="TRequest">The type of the request being processed.</typeparam>
/// <typeparam name="TResponse">The type of the response expected from the request.</typeparam>
/// <typeparam name="TException">The type of the exception being handled. Must derive from <see cref="Exception"/>.</typeparam>
public class MediatorExceptionLoggingHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TException : Exception
{
    private readonly ILoggingService _loggingService;

    public MediatorExceptionLoggingHandler(ILoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    public Task Handle(TRequest request, TException ex, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        _loggingService.LogError($"Something went wrong while handling request of type {typeof(TRequest)}", ex);

        Exception exception = ex;
        while (exception.InnerException != null)
        {
            exception = exception.InnerException;
            _loggingService.LogError(exception.Message, exception);
        }

        return Task.CompletedTask;
    }
}
