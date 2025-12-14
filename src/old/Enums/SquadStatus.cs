using System.Runtime.Serialization;

namespace servartur.Enums;

#pragma warning disable CA1515 // Consider making public types internal
public enum SquadStatus
#pragma warning restore CA1515 // Consider making public types internal
{
    [EnumMember(Value = "uknown")]
    Uknown,

    [EnumMember(Value = "upcoming")]
    Upcoming,

    [EnumMember(Value = "squadChoice")]
    SquadChoice,

    [EnumMember(Value = "submitted")]
    Submitted,

    [EnumMember(Value = "approved")]
    Approved,
    [EnumMember(Value = "rejected")]
    Rejected,

    [EnumMember(Value = "successful")]
    Successful,
    [EnumMember(Value = "failed")]
    Failed,
}
