using System.Reflection;
using System.ComponentModel;
using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.tests.Domain.Enums;

public class PersonIdentificationTypeTests
{
    [Theory]
    [InlineData(PersonIdentificationType.Unknown, 1)]
    [InlineData(PersonIdentificationType.IdentityDocument, 2)]
    [InlineData(PersonIdentificationType.Passport, 3)]
    [InlineData(PersonIdentificationType.RefugeeDocument, 4)]
    [InlineData(PersonIdentificationType.DigitalId, 5)]
    [InlineData(PersonIdentificationType.GcsId, 6)]
    public void EnumValues_AreCorrect(PersonIdentificationType type, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)type);
    }

    [Theory]
    [InlineData(PersonIdentificationType.Unknown, "Unknown")]
    [InlineData(PersonIdentificationType.IdentityDocument, "Identity Document")]
    [InlineData(PersonIdentificationType.Passport, "Passport")]
    [InlineData(PersonIdentificationType.RefugeeDocument, "Refugee Document")]
    [InlineData(PersonIdentificationType.DigitalId, "Digital ID")]
    [InlineData(PersonIdentificationType.GcsId, "GCS ID")]
    public void DescriptionAttribute_IsCorrect(PersonIdentificationType type, string expectedDescription)
    {
        var typeInfo = typeof(PersonIdentificationType);
        var memberInfo = typeInfo.GetMember(type.ToString()).First();
        var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();
        Assert.NotNull(descriptionAttribute);
        Assert.Equal(expectedDescription, descriptionAttribute.Description);
    }
}
