using System.Runtime.Serialization;

namespace servartur.Enums;

public enum RoomStatus
{
    [EnumMember(Value = "unknown")]
    Unknown,

    [EnumMember(Value = "matchup")]
    Matchup,

    [EnumMember(Value = "playing")]
    Playing,

    [EnumMember(Value = "assassination")]
    Assassination,

    [EnumMember(Value = "result")]
    Result,
}
