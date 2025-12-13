using System.Runtime.Serialization;

namespace servartur.Enums;

public enum Team
{
    [EnumMember(Value = "evil")]
    Evil,

    [EnumMember(Value = "good")]
    Good,
}
public enum Role
{
    [EnumMember(Value = "goodKnight")]
    GoodKnight,

    [EnumMember(Value = "evilEntity")]
    EvilEntity,

    [EnumMember(Value = "merlin")]
    Merlin,

    [EnumMember(Value = "assassin")]
    Assassin,

    [EnumMember(Value = "percival")]
    Percival,

    [EnumMember(Value = "morgana")]
    Morgana,

    [EnumMember(Value = "mordred")]
    Mordred,

    [EnumMember(Value = "oberon")]
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