using om.servicing.casemanagement.application.Features.OMCases.Commands;

namespace om.servicing.casemanagement.tests.Application.Features.OMCases.Commands;

public class CreateShellCaseCommandTests
{
    [Fact]
    public void Constructor_CanInstantiate()
    {
        var command = new CreateShellCaseCommand();
        Assert.NotNull(command);
    }
}
