using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using om.servicing.casemanagement.application;
using om.servicing.casemanagement.core;
using om.servicing.casemanagement.data;
using Serilog;
using System.Text.Json.Serialization;
using OM.RequestFramework.Infrastructure.Web.ErrorHandling;

namespace om.servicing.casemanagement.api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        RegisterServices(builder);

        var app = builder.Build();

        ConfigureApplication(app);
        app.Run();
    }

    private static void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger: loggingBuilder.ConfigureSerilog(builder.Environment.IsProduction()), dispose: true))
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddCors();
        builder.Services.AddHealthChecks();
        builder.Services.AddHttpLogging(httpLogging =>
        {
            httpLogging.LoggingFields = HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseBody;
            httpLogging.MediaTypeOptions.AddText("application/javascript");
            httpLogging.RequestBodyLogLimit = 4096;
            httpLogging.ResponseBodyLogLimit = 4096;
        });

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        ConfigureSwagger(builder.Services);
        ConfigureApiVersioning(builder.Services);


        // any additional customer services
        ConigureCustomServices(builder);
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services
            .AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CASE MANAGEMENT API", Version = "v1" });
                c.UseInlineDefinitionsForEnums();

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.CustomSchemaIds(type => type.FullName);
            });
    }

    private static void ConfigureApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
            options.SubstitutionFormat = "VVV";
        });
    }

    private static void ConigureCustomServices(WebApplicationBuilder builder)
    {
        builder.Services
                    .AddCaseManagementDataPostgres(builder.Configuration)
                    .RegisterApplicationFeatures()
                    .RegisterCustomServices();
    }

    private static void ConfigureApplication(WebApplication app)
    {
        Console.WriteLine($"Environment: {app.Environment.EnvironmentName} IsProduction: {app.Environment.IsProduction()} IsStaging: {app.Environment.IsStaging()}"); // This is production
        Console.WriteLine($"Environment directly from variable: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
        Console.WriteLine($"Logging verbosity ---> {Environment.GetEnvironmentVariable("LOGGING_VERBOSITY_LEVEL")}");

    #if DEBUG
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
            c.SerializeAsV2 = true;
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CASE MANAGEMENT API v1");
        });
    #else
                app.UseSwagger(c => {
                    c.RouteTemplate = "api/sips/swagger/{documentName}/swagger.json";
                    c.SerializeAsV2 = true;
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/api/sips/swagger/v1/swagger.json", "CASE MANAGEMENT v1");
                    c.RoutePrefix = "api/sips/swagger";
                });
    #endif
        app.UseDeveloperExceptionPage();

        app.Use(async (context, nextMiddleware) =>
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
                return Task.FromResult(0);
            });
            await nextMiddleware();
        });

        app.UseHttpsRedirection();

        app.UseHttpLogging();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseErrorHandlingMiddleware();
    }
}
