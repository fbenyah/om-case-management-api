using System.Reflection;
using System.ComponentModel;
using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.tests.Domain.Enums;

public class CaseChannelTests
{
    [Theory]
    [InlineData(CaseChannel.Unknown, 1)]
    [InlineData(CaseChannel.AgentWorkBench, 2)]
    [InlineData(CaseChannel.AdviserWorkBench, 3)]
    [InlineData(CaseChannel.Connect, 4)]
    [InlineData(CaseChannel.MomApp, 5)]
    [InlineData(CaseChannel.PublicWeb, 6)]
    [InlineData(CaseChannel.SecureWeb, 7)]
    [InlineData(CaseChannel.Branch, 8)]
    public void EnumValues_AreCorrect(CaseChannel channel, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)channel);
    }

    [Theory]
    [InlineData(CaseChannel.Unknown, "Unknown")]
    [InlineData(CaseChannel.AgentWorkBench, "UAW")]
    [InlineData(CaseChannel.AdviserWorkBench, "DAE")]
    [InlineData(CaseChannel.Connect, "Whatsapp")]
    [InlineData(CaseChannel.MomApp, "MomApp")]
    [InlineData(CaseChannel.PublicWeb, "Public Web")]
    [InlineData(CaseChannel.SecureWeb, "Secure Web")]
    [InlineData(CaseChannel.Branch, "Branch")]
    public void DescriptionAttribute_IsCorrect(CaseChannel channel, string expectedDescription)
    {
        var type = typeof(CaseChannel);
        var memberInfo = type.GetMember(channel.ToString()).First();
        var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();
        Assert.NotNull(descriptionAttribute);
        Assert.Equal(expectedDescription, descriptionAttribute.Description);
    }
}
