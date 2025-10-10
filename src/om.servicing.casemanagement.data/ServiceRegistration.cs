using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using om.servicing.casemanagement.data.Context;
using om.servicing.casemanagement.data.Repositories.Shared;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.data;

/// <summary>
/// Provides extension methods for registering services related to the Case Management system.
/// </summary>
/// <remarks>This static class contains methods to simplify the registration of services, such as database
/// contexts, into an application's dependency injection container.</remarks>
public static class ServiceRegistration
{
    /// <summary>
    /// This method adds the Case Management database context to the service collection using a PostgreSQL database.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCaseManagementDataPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<CaseManagerContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("CaseManagementDatabasePostgres"));
        });

        // Register the generic repository implementations
        RegisterRepositories<OMCase>(services);
        RegisterRepositories<OMInteraction>(services);
        RegisterRepositories<OMTransaction>(services);
        RegisterRepositories<OMTransactionType>(services);

        return services;
    }

    private static void RegisterRepositories<T>(this IServiceCollection services) where T : class
    {
        services.AddScoped<IGenericRepository<T>>(provider =>
                new GenericRepository<T, CaseManagerContext>(provider.GetRequiredService<CaseManagerContext>()));
    }
}
