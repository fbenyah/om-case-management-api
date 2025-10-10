using FluentAssertions;
using om.servicing.casemanagement.domain.Converters;
using System.ComponentModel;
using System.Text.Json;

namespace om.servicing.casemanagement.tests.Domain.Converters;

public class EnumDescriptionJsonConverterTests
{
    public enum TestStatus
    {
        [Description("Active status")]
        Active,
        [Description("Inactive status")]
        Inactive,
        Pending // No Description
    }    

    [Theory]
    [InlineData(TestStatus.Active, "\"Active status\"")]
    [InlineData(TestStatus.Inactive, "\"Inactive status\"")]
    [InlineData(TestStatus.Pending, "\"Pending\"")]
    public void Write_EnumValue_SerializesToDescriptionOrName(TestStatus value, string expectedJson)
    {
        var json = JsonSerializer.Serialize(value, GetOptions());
        json.Should().Be(expectedJson);
    }

    [Theory]
    [InlineData("\"Active status\"", TestStatus.Active)]
    [InlineData("\"Inactive status\"", TestStatus.Inactive)]
    public void Read_DescriptionOrName_DeserializesToEnum(string json, TestStatus expected)
    {
        var result = JsonSerializer.Deserialize<TestStatus>(json, GetOptions());
        result.Should().Be(expected);
    }

    [Fact]
    public void Read_UnknownDescription_ThrowsJsonException()
    {
        var json = "\"Unknown status\"";
        Action act = () => JsonSerializer.Deserialize<TestStatus>(json, GetOptions());
        act.Should().Throw<JsonException>()
            .WithMessage("Unknown description 'Unknown status' for TestStatus.");
    }

    private static JsonSerializerOptions GetOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new EnumDescriptionJsonConverter<TestStatus>());
        return options;
    }
}
