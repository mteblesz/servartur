using System.Runtime.Serialization;

namespace servartur.Enums;

public enum SquadStatus
{
    [EnumMember(Value = "Uknown")]
    Uknown,

    [EnumMember(Value = "Upcoming")]
    Upcoming,

    [EnumMember(Value = "SquadVoting")]
    SquadVoting,

    [EnumMember(Value = "Submitted")]
    Submitted,

    [EnumMember(Value = "Approved")]
    Approved,
    [EnumMember(Value = "Rejected")]
    Rejected,

    [EnumMember(Value = "QuestVoting")]
    QuestVoting,

    [EnumMember(Value = "Successful")]
    Successful,
    [EnumMember(Value = "Failed")]
    Failed,
}
