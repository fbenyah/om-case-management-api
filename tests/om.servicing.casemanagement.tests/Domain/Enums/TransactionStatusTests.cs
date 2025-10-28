using om.servicing.casemanagement.domain.Enums;
using System.ComponentModel;
using System.Reflection;

namespace om.servicing.casemanagement.tests.Domain.Enums;

public class TransactionStatusTests
{
    [Fact]
    public void Enum_HasExpectedNamesAndCount()
    {
        var names = Enum.GetNames(typeof(TransactionStatus));
        Assert.Equal(new[] { "Unknown", "Aborted", "Submitted", "InProgress", "Cancelled", "Closed", "Received" }, names);
    }

    [Fact]
    public void Enum_HasExpectedNumericValues()
    {
        Assert.Equal(1, (int)TransactionStatus.Unknown);
        Assert.Equal(2, (int)TransactionStatus.Aborted);
        Assert.Equal(3, (int)TransactionStatus.Submitted);
        Assert.Equal(4, (int)TransactionStatus.InProgress);
        Assert.Equal(5, (int)TransactionStatus.Cancelled);
        Assert.Equal(6, (int)TransactionStatus.Closed);
        Assert.Equal(7, (int)TransactionStatus.Received);
    }

    private static string? GetDescription(TransactionStatus status)
    {
        var member = typeof(TransactionStatus).GetMember(status.ToString()).FirstOrDefault();
        var attr = member?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description;
    }

    [Fact]
    public void DescriptionAttributes_AreCorrect()
    {
        Assert.Equal("Unknown", GetDescription(TransactionStatus.Unknown));
        Assert.Equal("Aborted", GetDescription(TransactionStatus.Aborted));
        Assert.Equal("Submitted", GetDescription(TransactionStatus.Submitted));
        Assert.Equal("InProgress", GetDescription(TransactionStatus.InProgress));
        Assert.Equal("Cancelled", GetDescription(TransactionStatus.Cancelled));
        Assert.Equal("Closed", GetDescription(TransactionStatus.Closed));
        Assert.Equal("Received", GetDescription(TransactionStatus.Received));
    }
}
