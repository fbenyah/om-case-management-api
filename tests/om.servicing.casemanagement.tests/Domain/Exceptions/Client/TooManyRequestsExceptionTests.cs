using FluentAssertions;
using om.servicing.casemanagement.domain.Exceptions.Client;

namespace om.servicing.casemanagement.tests.Domain.Exceptions.Client;

public class TooManyRequestsExceptionTests
{
    [Fact]
    public void Constructor_DefaultMessage_SetsExpectedValues()
    {
        var exception = new TooManyRequestsException();

        exception.Message.Should().Be("Invalid Request");
        exception.HttpResponseCode.Should().Be(429);
    }

    [Fact]
    public void Constructor_CustomMessage_SetsMessage()
    {
        var customMessage = "Custom too many requests error";
        var exception = new TooManyRequestsException(customMessage);

        exception.Message.Should().Be(customMessage);
        exception.HttpResponseCode.Should().Be(429);
    }
}
