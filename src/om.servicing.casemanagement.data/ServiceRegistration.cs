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

    /// <summary>
    /// Registers a scoped generic repository for the specified entity type in the service collection.
    /// </summary>
    /// <remarks>This method registers an implementation of <see cref="IGenericRepository{T}"/> that uses 
    /// <see cref="CaseManagerContext"/> as the database context. The repository is registered with  a scoped lifetime,
    /// meaning a new instance will be created for each request.</remarks>
    /// <typeparam name="T">The type of the entity for which the repository is being registered. Must be a reference type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the repository will be added.</param>
    private static void RegisterRepositories<T>(this IServiceCollection services) where T : class
    {
        services.AddScoped<IGenericRepository<T>>(provider =>
                new GenericRepository<T, CaseManagerContext>(provider.GetRequiredService<CaseManagerContext>()));
    }
}
