using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.WithCaller;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Json;
using System.Reflection;

namespace om.servicing.casemanagement.core;

public static class ServiceRegistration
{
    public static Serilog.ILogger ConfigureSerilog(this ILoggingBuilder loggingBuilder, bool isProduction)
    {
        LoggerConfiguration logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithCaller()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers())
            .Enrich.WithDemystifiedStackTraces()
            .Enrich.WithProperty("Application", Assembly.GetCallingAssembly().GetName().Name)
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development")
            .WriteTo.Console();

        if (!isProduction)
        {
            logger.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [#{ThreadId}] {Message:lj}{Exception}{Properties:j}{NewLine}",
                                        restrictedToMinimumLevel: LogEventLevel.Warning);
        }
        else
        {
            logger.WriteTo.Console(new JsonFormatter(renderMessage: true), restrictedToMinimumLevel: LogEventLevel.Warning);
        }

        logger.WriteTo.File(new JsonFormatter(renderMessage: true),
                Path.Combine("logs", $"{Assembly.GetCallingAssembly().GetName().Name}.log"),
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 3);

        return logger.CreateLogger();
    }
}
