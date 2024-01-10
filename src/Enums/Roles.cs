using System.Runtime.Serialization;

namespace servartur.Enums;

public enum Team
{
    [EnumMember(Value = "Evil")]
    Evil,

    [EnumMember(Value = "Good")]
    Good,
}
public enum Role
{
    [EnumMember(Value = "GoodKnight")]
    GoodKnight,

    [EnumMember(Value = "EvilEntity")]
    EvilEntity,

    [EnumMember(Value = "Merlin")]
    Merlin,

    [EnumMember(Value = "Assassin")]
    Assassin,

    [EnumMember(Value = "Percival")]
    Percival,

    [EnumMember(Value = "Morgana")]
    Morgana,

    [EnumMember(Value = "Mordred")]
    Mordred,

    [EnumMember(Value = "Oberon")]
    Oberon,
}
public static class RoleTeamMapping
{
    public static Team Map(Role role) => _dictionary[role];

    private static readonly Dictionary<Role, Team> _dictionary = new()
    {
        { Role.GoodKnight, Team.Good },
        { Role.EvilEntity, Team.Evil },

        { Role.Merlin, Team.Good },
        { Role.Assassin, Team.Evil },

        { Role.Percival, Team.Good },
        { Role.Morgana, Team.Evil },

        { Role.Mordred, Team.Evil },
        { Role.Oberon, Team.Evil },
    };
}