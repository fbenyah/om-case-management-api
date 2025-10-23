using System.ComponentModel;

namespace om.servicing.casemanagement.domain.Enums;

public enum InteractionStatus
{
    [Description("Unknown")]
    Unknown = 1,

    [Description("Initiated")]
    Initiated,

    [Description("InProgress")]
    InProgress,

    [Description("Closed")]
    Closed
}
