using System.ComponentModel;

namespace om.servicing.casemanagement.domain.Enums;

public enum PersonIdentificationType
{
    [Description("Unknown")]
    Unknown = 1,

    [Description("Identity Document")]
    IdentityDocument,

    [Description("Passport")]
    Passport,

    [Description("Refugee Document")]
    RefugeeDocument,

    [Description("Digital ID")]
    DigitalId,

    [Description("GCS ID")]
    GcsId
}
