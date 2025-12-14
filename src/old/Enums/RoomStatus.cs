using System.Runtime.Serialization;

namespace servartur.Enums;

#pragma warning disable CA1515 // Consider making public types internal
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

    [EnumMember(Value = "resultEvilWin")]
    ResultGoodWin,

    [EnumMember(Value = "resultGoodWin")]
    ResultEvilWin,
}
