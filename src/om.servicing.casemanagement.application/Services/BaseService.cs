using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.application.Services;

public abstract class BaseService
{
    protected readonly ILoggingService _loggingService;

    public BaseService(ILoggingService loggingService)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    }
}
