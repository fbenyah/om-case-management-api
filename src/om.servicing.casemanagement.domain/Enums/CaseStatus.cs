using System.ComponentModel;

namespace om.servicing.casemanagement.domain.Enums;

public enum CaseStatus
{
    [Description("Unknown")]
    Unknown = 1,

    [Description("Initiated")]
    Initiated,

    [Description("Open")]
    Open,

    [Description("InProgress")]
    InProgress,

    [Description("Closed")]
    Closed
}
