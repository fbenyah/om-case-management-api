using om.servicing.casemanagement.domain.Enums;
using System.ComponentModel;
using System.Reflection;

namespace om.servicing.casemanagement.tests.Domain.Enums;

public class InteractionStatusTests
{
    [Fact]
    public void Enum_HasExpectedNamesAndCount()
    {
        var names = Enum.GetNames(typeof(InteractionStatus));
        Assert.Equal(new[] { "Unknown", "Initiated", "InProgress", "Closed" }, names);
    }

    [Fact]
    public void Enum_HasExpectedNumericValues()
    {
        Assert.Equal(1, (int)InteractionStatus.Unknown);
        Assert.Equal(2, (int)InteractionStatus.Initiated);
        Assert.Equal(3, (int)InteractionStatus.InProgress);
        Assert.Equal(4, (int)InteractionStatus.Closed);
    }

    private static string? GetDescription(InteractionStatus status)
    {
        var member = typeof(InteractionStatus).GetMember(status.ToString()).FirstOrDefault();
        var attr = member?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description;
    }

    [Fact]
    public void DescriptionAttributes_AreCorrect()
    {
        Assert.Equal("Unknown", GetDescription(InteractionStatus.Unknown));
        Assert.Equal("Initiated", GetDescription(InteractionStatus.Initiated));
        Assert.Equal("InProgress", GetDescription(InteractionStatus.InProgress));
        Assert.Equal("Closed", GetDescription(InteractionStatus.Closed));
    }
}
