using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.domain.Core;

/// <summary>
/// This class serves as a base for all case management related classes, providing common properties such as Id, CreatedDate, UpdateDate, Status, and Channel.
/// </summary>
public abstract class BaseOmCase
{
    public string Id { get; set; } = Ulid.NewUlid().ToString();
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime? UpdateDate { get; set; } = null;
    public CaseStatus Status { get; set; } = CaseStatus.Open;
    public CaseChannel Channel { get; set; } = CaseChannel.Unknown;
}
