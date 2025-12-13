using System.Runtime.Serialization;

namespace servartur.Enums;

internal enum RoomStatus
{
    [EnumMember(Value = "unknown")]
    Unknown,

    [EnumMember(Value = "matchup")]
    Matchup,

    [EnumMember(Value = "playing")]
    Playing,

    [EnumMember(Value = "assassination")]
    Assassination,

    [EnumMember(Value = "resultEvilWin")]
    ResultGoodWin,

    [EnumMember(Value = "resultGoodWin")]
    ResultEvilWin,
}
