using System.Reflection;
using System.ComponentModel;
using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.tests.Domain.Enums;

public class CaseStatusTests
{
    [Theory]
    [InlineData(CaseStatus.Unknown, 1)]
    [InlineData(CaseStatus.Initiated, 2)]
    [InlineData(CaseStatus.Open, 3)]
    [InlineData(CaseStatus.InProgress, 4)]
    [InlineData(CaseStatus.Closed, 5)]
    public void EnumValues_AreCorrect(CaseStatus status, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)status);
    }

    [Theory]
    [InlineData(CaseStatus.Unknown, "Unknown")]
    [InlineData(CaseStatus.Initiated, "Initiated")]
    [InlineData(CaseStatus.Open, "Open")]
    [InlineData(CaseStatus.InProgress, "InProgress")]
    [InlineData(CaseStatus.Closed, "Closed")]
    public void DescriptionAttribute_IsCorrect(CaseStatus status, string expectedDescription)
    {
        var type = typeof(CaseStatus);
        var memberInfo = type.GetMember(status.ToString()).First();
        var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();
        Assert.NotNull(descriptionAttribute);
        Assert.Equal(expectedDescription, descriptionAttribute.Description);
    }
}
