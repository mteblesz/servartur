using System.Runtime.Serialization;

namespace servartur.Enums;

public enum SquadStatus
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

    [EnumMember(Value = "questVoting")]
    QuestVoting,

    [EnumMember(Value = "successful")]
    Successful,
    [EnumMember(Value = "failed")]
    Failed,
}
