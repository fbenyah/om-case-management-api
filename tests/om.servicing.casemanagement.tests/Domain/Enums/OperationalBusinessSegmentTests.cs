using om.servicing.casemanagement.domain.Enums;
using System.ComponentModel;
using System.Reflection;

namespace om.servicing.casemanagement.tests.Domain.Enums;

public class OperationalBusinessSegmentTests
{
    [Fact]
    public void Enum_HasExpectedValues()
    {
        Assert.Equal(1, (int)OperationalBusinessSegment.Unknown);
        Assert.Equal(2, (int)OperationalBusinessSegment.CustomerServicing);
    }

    [Theory]
    [InlineData(OperationalBusinessSegment.Unknown, "Unknown")]
    [InlineData(OperationalBusinessSegment.CustomerServicing, "Customer Servicing")]
    public void Enum_HasExpectedDescriptionAttribute(OperationalBusinessSegment segment, string expectedDescription)
    {
        var type = typeof(OperationalBusinessSegment);
        var memberInfo = type.GetMember(segment.ToString()).First();
        var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();
        Assert.NotNull(descriptionAttribute);
        Assert.Equal(expectedDescription, descriptionAttribute.Description);
    }
}
