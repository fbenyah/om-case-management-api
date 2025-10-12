namespace om.servicing.casemanagement.domain.Configuration;

/// <summary>
/// Provides a set of predefined environment names commonly used in application lifecycle management.
/// </summary>
/// <remarks>The <see cref="Environments"/> class defines constants representing typical application environments,
/// such as <see cref="Development"/>, <see cref="Staging"/>, and <see cref="Production"/>. These values can be used to
/// configure or identify the current runtime environment of an application.</remarks>
public static class Environments
{
    public const string Development = "Development";
    public const string Staging = "Staging";
    public const string Production = "Production";
}

/// <summary>
/// Provides constants representing common deployment environments.
/// </summary>
/// <remarks>This class defines string constants for standard deployment environments, such as  development,
/// quality assurance, and production. These constants can be used to  standardize environment identifiers across
/// applications.</remarks>
public static class DeploymentEnvironments
{
    public const string Development = "dev";
    public const string QualityAssurance = "qa";
    public const string Production = "prod";
}

public static class Logging
{
    public const string LoggingVerbosityLevel = "LOGGING_VERBOSITY_LEVEL";
}
