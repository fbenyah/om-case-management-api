using System.ComponentModel;

namespace om.servicing.casemanagement.domain.Enums;

public enum CaseChannel
{
    [Description("Unknown")]
    Unknown = 1,

    [Description("UAW")]
    AgentWorkBench,

    [Description("DAE")]
    AdviserWorkBench,

    [Description("Whatsapp")]
    Connect,

    [Description("MomApp")]
    MomApp,

    [Description("Public Web")]
    PublicWeb,

    [Description("Secure Web")]
    SecureWeb,

    [Description("Branch")]
    Branch
}
