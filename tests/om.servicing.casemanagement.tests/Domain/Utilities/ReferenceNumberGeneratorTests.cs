using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Utilities;
using System.Reflection;

namespace om.servicing.casemanagement.tests.Domain.Utilities;

public class ReferenceNumberGeneratorTests
{
    [Fact]
    public void GenerateReferenceNumber_ValidInput_ReturnsExpectedFormat()
    {
        // Arrange
        string ulid = "01H9Z6G7YB2XK3VQ5F4J8T"; // 22 chars, last 6: "5F4J8T"
        var channel = CaseChannel.PublicWeb;
        var obs = OperationalBusinessSegment.CustomerServicing;

        // Act
        var reference = ReferenceNumberGenerator.GenerateReferenceNumber(ulid, channel, obs);

        // Assert
        Assert.StartsWith("CSP", reference); // "CS" + "P"
        Assert.Equal(18, reference.Length); // "CS" + "P" + "yyMMdd" + "random(3)" + "ulid(6)"
        Assert.EndsWith("5F4J8T", reference);
    }

    [Fact]
    public void GenerateReferenceNumber_DefaultOBS_ReturnsExpectedPrefix()
    {
        string ulid = "01H9Z6G7YB2XK3VQ5F4J8T";
        var channel = CaseChannel.AgentWorkBench;

        var reference = ReferenceNumberGenerator.GenerateReferenceNumber(ulid, channel);

        Assert.StartsWith("CST", reference); // "CS" + "T"
    }

    [Theory]
    [InlineData(null, CaseChannel.PublicWeb)]
    [InlineData("", CaseChannel.PublicWeb)]
    [InlineData("   ", CaseChannel.PublicWeb)]
    [InlineData("12345", CaseChannel.PublicWeb)] // less than 6 chars
    [InlineData("01H9Z6G7YB2XK3VQ5F4J8T", CaseChannel.Unknown)]
    public void GenerateReferenceNumber_InvalidInput_ThrowsArgumentException(string ulid, CaseChannel channel)
    {
        Assert.Throws<ArgumentException>(() =>
            ReferenceNumberGenerator.GenerateReferenceNumber(ulid, channel));
    }

    [Fact]
    public void GenerateReferenceNumber_UnknownOBS_ThrowsArgumentOutOfRangeException()
    {
        string ulid = "01H9Z6G7YB2XK3VQ5F4J8T";
        var channel = CaseChannel.PublicWeb;
        var obs = OperationalBusinessSegment.Unknown;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ReferenceNumberGenerator.GenerateReferenceNumber(ulid, channel, obs));
    }

    [Fact]
    public void GenerateReferenceNumber_UnknownChannel_ThrowsArgumentOutOfRangeException()
    {
        string ulid = "01H9Z6G7YB2XK3VQ5F4J8T";
        var obs = OperationalBusinessSegment.CustomerServicing;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ReferenceNumberGenerator.GenerateReferenceNumber(ulid, (CaseChannel)999, obs));
    }

    [Theory]
    [InlineData(CaseChannel.AdviserWorkBench, "D")]
    [InlineData(CaseChannel.AgentWorkBench, "T")]
    [InlineData(CaseChannel.Branch, "B")]
    [InlineData(CaseChannel.Connect, "C")]
    [InlineData(CaseChannel.MomApp, "A")]
    [InlineData(CaseChannel.PublicWeb, "P")]
    [InlineData(CaseChannel.SecureWeb, "W")]
    public void GetChannelPrefix_ValidChannel_ReturnsExpectedPrefix(CaseChannel channel, string expectedPrefix)
    {
        var method = typeof(ReferenceNumberGenerator)
            .GetMethod("GetChannelPrefix", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        var result = method.Invoke(null, new object[] { channel }) as string;

        Assert.Equal(expectedPrefix, result);
    }

    [Fact]
    public void GetChannelPrefix_UnknownChannel_ThrowsArgumentOutOfRangeException()
    {
        var method = typeof(ReferenceNumberGenerator)
            .GetMethod("GetChannelPrefix", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        var ex = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, new object[] { CaseChannel.Unknown }));

        Assert.IsType<ArgumentOutOfRangeException>(ex.InnerException);
    }

    [Fact]
    public void GetChannelPrefix_InvalidChannel_ThrowsArgumentOutOfRangeException()
    {
        var method = typeof(ReferenceNumberGenerator)
            .GetMethod("GetChannelPrefix", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        var ex = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, new object[] { (CaseChannel)999 }));

        Assert.IsType<ArgumentOutOfRangeException>(ex.InnerException);
    }
}
