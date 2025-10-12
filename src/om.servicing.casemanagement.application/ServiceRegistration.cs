using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using om.servicing.casemanagement.application.Features.Configuration;
using om.servicing.casemanagement.application.Services;

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
        // Register application services here
        services.AddScoped<IOMCaseService, OMCaseService>();

        return services;
    }
}
