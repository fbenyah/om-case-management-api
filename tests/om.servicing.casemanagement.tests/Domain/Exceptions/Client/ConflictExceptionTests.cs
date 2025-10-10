using FluentAssertions;
using om.servicing.casemanagement.domain.Exceptions.Client;

namespace om.servicing.casemanagement.tests.Domain.Exceptions.Client;

public class ConflictExceptionTests
{
    [Fact]
    public void Constructor_DefaultMessage_SetsExpectedValues()
    {
        var exception = new ConflictException();

        exception.Message.Should().Be("Invalid Request");
        exception.HttpResponseCode.Should().Be(409);
    }

    [Fact]
    public void Constructor_CustomMessage_SetsMessage()
    {
        var customMessage = "Custom conflict error";
        var exception = new ConflictException(customMessage);

        exception.Message.Should().Be(customMessage);
        exception.HttpResponseCode.Should().Be(409);
    }
}
