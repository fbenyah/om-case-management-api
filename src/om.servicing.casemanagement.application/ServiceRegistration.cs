using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using om.servicing.casemanagement.application.Features.Configuration;
using om.servicing.casemanagement.application.Services;
using OM.RequestFramework.Clients;
using OM.RequestFramework.Infrastructure.Common.Config;

namespace om.servicing.casemanagement.application;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterApplicationFeatures(this IServiceCollection services)
    {
        return services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatorLoggingBehaviour<,>))
                .AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(MediatorExceptionLoggingHandler<,,>));
    }

    public static IServiceCollection RegisterCustomServices(this IServiceCollection services)
    {        
        services
            // Register shared services here (SAE Request Framework)
            .AddApplicationLogging()
            .AddTransient<CircuitBreakerAndRetryHandler>()
            .AddTransient<IHttpMessageHandlerBuilderFilter, CustomMessageHandlerBuilderFilter>()


            // Register application services here
            .AddScoped<IOMCaseService, OMCaseService>();

        return services;
    }
}
