using Moq;
using om.servicing.casemanagement.application.Features;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features;

public class SharedFeaturesTests
{
    [Fact]
    public void Constructor_SetsLoggingService()
    {
        var loggingServiceMock = new Mock<ILoggingService>();
        var testFeature = new TestSharedFeatures(loggingServiceMock.Object);

        Assert.NotNull(testFeature.GetLoggingService());
        Assert.Equal(loggingServiceMock.Object, testFeature.GetLoggingService());
    }
}

public class TestSharedFeatures : SharedFeatures
{
    public TestSharedFeatures(ILoggingService loggingService) : base(loggingService) { }
    public ILoggingService GetLoggingService() => _loggingService;
}
