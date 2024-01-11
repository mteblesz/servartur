using servartur.Entities;
using servartur.Exceptions;
using servartur.Enums;
using servartur.Algorithms;
using Humanizer;

namespace servartur.DomainLogic;

public static class GameStartHelper
{
    public class RoleInfo
    {
        public bool AreMerlinAndAssassinInGame { get; set; }
        public bool ArePercivalAreMorganaInGame { get; set; }
        public bool AreOberonAndMordredInGame { get; set; }

        public RoleInfo(
            bool areMerlinAndAssassinInGame,
            bool arePercivalAreMorganaInGame,
            bool areOberonAndMordredInGame)
        {
            AreMerlinAndAssassinInGame = areMerlinAndAssassinInGame;
            ArePercivalAreMorganaInGame = arePercivalAreMorganaInGame;
            AreOberonAndMordredInGame = areOberonAndMordredInGame;
        }
    }
    public static List<Role> MakeRoleDeck(int playersCount, RoleInfo roleInfo, out bool tooManyEvilRoles)
    {
        if (!GameCountsCalculator.IsPlayerCountValid(playersCount))
            throw new ArgumentException("Invalid number of players given");

        var roles = new List<Role>();
        if (roleInfo.AreMerlinAndAssassinInGame)
            roles.AddRange([Role.Merlin, Role.Assassin]);
        if (roleInfo.ArePercivalAreMorganaInGame)
            roles.AddRange([Role.Percival, Role.Morgana]);
        if (roleInfo.AreOberonAndMordredInGame)
            roles.AddRange([Role.Oberon, Role.Mordred]);

        int evilRolesCount = roles.Count(r => RoleTeamMapping.Map(r) == Team.Evil);
        int goodRolesCount = roles.Count(r => RoleTeamMapping.Map(r) == Team.Good);
        int evilPlayersTargetCount = GameCountsCalculator.GetEvilPlayersCount(playersCount);
        int goodPlayersTargetCount = playersCount - evilPlayersTargetCount;

        tooManyEvilRoles = evilRolesCount > evilPlayersTargetCount;
        if (tooManyEvilRoles) return [];

        roles.AddRange(Enumerable.Repeat(Role.EvilEntity, evilPlayersTargetCount - evilRolesCount));
        roles.AddRange(Enumerable.Repeat(Role.GoodKnight, goodPlayersTargetCount - goodRolesCount));

        roles.Shuffle();
        return roles;
    }
    public static Squad MakeFirstSquad(Player leader, int playersCount)
    {
        if (!GameCountsCalculator.IsPlayerCountValid(playersCount))
            throw new ArgumentException("Invalid number of players given");

        var questNumber = 1;
        var firstSquadSize = GameCountsCalculator.GetSquadRequiredSize(playersCount, questNumber);
        var firstSquad = new Squad()
        {
            Leader = leader,
            QuestNumber = questNumber,
            RoundNumber = 1,
            RequiredPlayersNumber = firstSquadSize,
            Status = SquadStatus.SquadVoting,
        };
        return firstSquad;
    }
}