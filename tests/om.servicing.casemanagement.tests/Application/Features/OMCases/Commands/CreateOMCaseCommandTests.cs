using om.servicing.casemanagement.application.Features.OMCases.Commands;
using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Commands;

public class CreateOMCaseCommandTests
{
    [Fact]
    public void Constructor_InitializesPropertiesToDefaults()
    {
        var command = new CreateOMCaseCommand();
        Assert.NotNull(command);
        Assert.Equal(CaseChannel.Unknown, command.SourceChannel);
        Assert.Equal(string.Empty, command.IdentificationNumber);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var command = new CreateOMCaseCommand
        {
            SourceChannel = CaseChannel.PublicWeb,
            IdentificationNumber = "ID123"
        };
        Assert.Equal(CaseChannel.PublicWeb, command.SourceChannel);
        Assert.Equal("ID123", command.IdentificationNumber);
    }
}
