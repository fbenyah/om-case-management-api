using MediatR.Pipeline;
using Moq;
using om.servicing.casemanagement.application.Features.Configuration;
using om.servicing.casemanagement.tests.Shared.Models;
using OM.RequestFramework.Core.Logging;

namespace om.servicing.casemanagement.tests.Application.Features.Configuration;

public class MediatorExceptionLoggingHandlerTests
{
    [Fact]
    public async Task Handle_LogsExceptionAndInnerExceptions()
    {
        // Arrange
        var loggingServiceMock = new Mock<ILoggingService>();

        var innerMostException = new InvalidOperationException("Innermost exception");
        var innerException = new Exception("Inner exception", innerMostException);
        var mainException = new Exception("Main exception", innerException);

        var handler = new MediatorExceptionLoggingHandler<DummyRequest, string, Exception>(loggingServiceMock.Object);

        var request = new DummyRequest();
        var state = new RequestExceptionHandlerState<string>();

        // Act
        await handler.Handle(request, mainException, state, CancellationToken.None);

        // Assert
        loggingServiceMock.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("Something went wrong while handling request")), mainException),
            Times.Once);

        loggingServiceMock.Verify(
            x => x.LogError("Inner exception", innerException),
            Times.Once);

        loggingServiceMock.Verify(
            x => x.LogError("Innermost exception", innerMostException),
            Times.Once);
    }

    [Fact]
    public async Task Handle_LogsOnlyMainException_WhenNoInnerException()
    {
        // Arrange
        var loggingServiceMock = new Mock<ILoggingService>();
        var mainException = new Exception("Main exception");

        var handler = new MediatorExceptionLoggingHandler<DummyRequest, string, Exception>(loggingServiceMock.Object);

        var request = new DummyRequest();
        var state = new RequestExceptionHandlerState<string>();

        // Act
        await handler.Handle(request, mainException, state, CancellationToken.None);

        // Assert
        loggingServiceMock.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("Something went wrong while handling request")), mainException),
            Times.Once);

        // Should not log any inner exception
        loggingServiceMock.Verify(
            x => x.LogError(It.IsAny<string>(), It.Is<Exception>(ex => ex != mainException)),
            Times.Never);
    }
}
