using System.ComponentModel;

namespace om.servicing.casemanagement.domain.Enums;

public enum TransactionStatus
{
    [Description("Unknown")]
    Unknown = 1,

    [Description("Aborted")]
    Aborted,

    [Description("Submitted")]
    Submitted,

    [Description("InProgress")]
    InProgress,

    [Description("Cancelled")]
    Cancelled,

    [Description("Closed")]
    Closed
}
