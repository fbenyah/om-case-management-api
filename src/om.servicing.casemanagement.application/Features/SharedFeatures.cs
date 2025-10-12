using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Features;

public abstract class SharedFeatures
{
    internal readonly ILoggingService _loggingService;

    public SharedFeatures(
            ILoggingService loggingService
        )
    {
        _loggingService = loggingService;
    }
}