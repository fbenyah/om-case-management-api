using Microsoft.Extensions.DependencyInjection;
using om.servicing.casemanagement.application.Services;

namespace om.servicing.casemanagement.application;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterCustomServices(this IServiceCollection services)
    {
        // Register application services here
        services.AddScoped<IOMCaseService, OMCaseService>();

        return services;
    }
}
