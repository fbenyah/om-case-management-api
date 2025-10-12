namespace om.servicing.casemanagement.domain.Configuration.Options;

/// <summary>
/// Represents configuration options for managing application environments, including environment name, variant,
/// deployment environment, and production status.
/// </summary>
/// <remarks>This class provides properties to configure and retrieve details about the application's current
/// environment. The <see cref="EnvironmentName"/> property determines the environment type (e.g., Development, Staging,
/// Production) and updates related properties such as <see cref="DeploymentEnvironment"/> and <see cref="IsNonProd"/>
/// accordingly.</remarks>
public class EnvironmentOptions
{
    private string _environmentName = string.Empty;
    public string EnvironmentName { get { return _environmentName; } set { SetEnvironmentNameAndUpdateOptions(value); } }
    public string? EnvironmentVariant { get; set; }
    public string DeploymentEnvironment { get; private set; } = string.Empty;

    public bool IsNonProd { get; private set; }

    private void SetEnvironmentNameAndUpdateOptions(string environmentName)
    {
        _environmentName = environmentName;

        IsNonProd = Environments.Development.Equals(environmentName, StringComparison.InvariantCultureIgnoreCase) ||
            Environments.Staging.Equals(environmentName, StringComparison.InvariantCultureIgnoreCase);

        switch (environmentName)
        {
            case Environments.Development:
                DeploymentEnvironment = DeploymentEnvironments.Development;
                break;
            case Environments.Staging:
                DeploymentEnvironment = DeploymentEnvironments.QualityAssurance;
                break;
            case Environments.Production:
                DeploymentEnvironment = DeploymentEnvironments.Production;
                break;
            default:
                DeploymentEnvironment = string.Empty;
                break;
        }
    }
}
