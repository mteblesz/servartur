using servartur.Enums;
using servartur.Utils;

namespace servartur.DomainLogic;

#pragma warning disable CA1515 // Consider making public types internal
public class RoleInfo
{
    public bool AreMerlinAndAssassinInGame { get; set; }
    public bool ArePercivalAndMorganaInGame { get; set; }
    public bool AreOberonAndMordredInGame { get; set; }

    public RoleInfo(
        bool areMerlinAndAssassinInGame,
        bool arePercivalAndMorganaInGame,
        bool areOberonAndMordredInGame)
    {
        AreMerlinAndAssassinInGame = areMerlinAndAssassinInGame;
        ArePercivalAndMorganaInGame = arePercivalAndMorganaInGame;
        AreOberonAndMordredInGame = areOberonAndMordredInGame;
    }
}

internal static class GameStartHelper
{
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA1021 // Avoid out parameters
    public static List<Role> MakeRoleDeck(int playerCount, RoleInfo roleInfo, out bool tooManyEvilRoles)
#pragma warning restore CA1021 // Avoid out parameters
    {
        if (!GameCountsCalculator.IsPlayerCountValid(playerCount))
        {
            throw new ArgumentException("Invalid number of players given");
        }

        var roles = new List<Role>();
        if (roleInfo.AreMerlinAndAssassinInGame)
        {
            roles.AddRange([Role.Merlin, Role.Assassin]);
        }

        if (roleInfo.ArePercivalAndMorganaInGame)
        {
            roles.AddRange([Role.Percival, Role.Morgana]);
        }

        if (roleInfo.AreOberonAndMordredInGame)
        {
            roles.AddRange([Role.Oberon, Role.Mordred]);
        }

        var evilRolesCount = roles.Count(r => RoleTeamMapping.Map(r) == Team.Evil);
        var goodRolesCount = roles.Count(r => RoleTeamMapping.Map(r) == Team.Good);
        var evilPlayersTargetCount = GameCountsCalculator.GetEvilPlayerCount(playerCount);
        var goodPlayersTargetCount = playerCount - evilPlayersTargetCount;

        tooManyEvilRoles = evilRolesCount > evilPlayersTargetCount;
        if (tooManyEvilRoles)
        {
            return [];
        }

        roles.AddRange(Enumerable.Repeat(Role.EvilEntity, evilPlayersTargetCount - evilRolesCount));
        roles.AddRange(Enumerable.Repeat(Role.GoodKnight, goodPlayersTargetCount - goodRolesCount));

        roles.Shuffle();
        return roles;
    }
}
