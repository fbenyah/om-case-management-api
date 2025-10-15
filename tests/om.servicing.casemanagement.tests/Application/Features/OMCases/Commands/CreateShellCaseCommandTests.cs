using om.servicing.casemanagement.application.Features.OMCases.Commands;
using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Commands;

public class CreateShellCaseCommandTests
{
    [Fact]
    public void Constructor_CanInstantiate()
    {
        var command = new CreateShellCaseCommand();
        Assert.Equal(CaseChannel.Unknown, command.SourceChannel);
        Assert.NotNull(command);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var command = new CreateShellCaseCommand
        {
            SourceChannel = CaseChannel.PublicWeb
        };
        Assert.Equal(CaseChannel.PublicWeb, command.SourceChannel);
    }
}
