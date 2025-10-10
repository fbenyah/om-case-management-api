using om.servicing.casemanagement.domain.Utilities;

namespace om.servicing.casemanagement.tests.Domain.Utilities;

public class UlidUtilsTests
{
    [Fact]
    public void NewUlidString_ReturnsValidUlidString()
    {
        var ulidStr = UlidUtils.NewUlidString();
        Assert.True(UlidUtils.IsValidUlid(ulidStr));
        Assert.Equal(26, ulidStr.Length);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-ulid")]
    [InlineData("1234567890123456789012345")] // 25 chars
    [InlineData("123456789012345678901234567")] // 27 chars
    public void IsValidUlid_ReturnsFalseForInvalid(string input)
    {
        Assert.False(UlidUtils.IsValidUlid(input));
    }

    [Fact]
    public void IsValidUlid_ReturnsTrueForValidUlid()
    {
        var ulidStr = UlidUtils.NewUlidString();
        Assert.True(UlidUtils.IsValidUlid(ulidStr));
    }

    [Fact]
    public void TryParseUlid_ReturnsTrueAndParsesValidUlid()
    {
        var ulidStr = UlidUtils.NewUlidString();
        var result = UlidUtils.TryParseUlid(ulidStr, out var ulid);
        Assert.True(result);
        Assert.Equal(ulidStr, ulid.ToString());
    }

    [Fact]
    public void TryParseUlid_ReturnsFalseAndDefaultForInvalidUlid()
    {
        var result = UlidUtils.TryParseUlid("invalid-ulid", out var ulid);
        Assert.False(result);
        Assert.Equal(default, ulid);
    }

    [Fact]
    public void UlidToGuid_ConvertsUlidToGuidCorrectly()
    {
        var ulid = Ulid.NewUlid();
        var guid = UlidUtils.UlidToGuid(ulid);
        Assert.IsType<Guid>(guid);
        // The conversion should be reversible
        Assert.Equal(ulid, Ulid.Parse(ulid.ToString()));
    }
}
