using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features;

public abstract class SharedFeatures
{
    protected internal readonly ILoggingService _loggingService;

    protected readonly string[]? InteractionsTransactionsIncludeNavigationProperties = new[] { "Interactions", "Interactions.Transactions" };
    protected readonly string[]? TransactionsIncludeNavigationProperties = new[] { "Transactions", };

    public SharedFeatures(
            ILoggingService loggingService
        )
    {
        _loggingService = loggingService;
    }
}