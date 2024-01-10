using servartur.Entities;
using servartur.Exceptions;
using servartur.Enums;
using servartur.Algorithms;

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
    public static List<Role> MakeRoleDeck(RoleInfo roleInfo, int playersCount, out bool tooManyEvilPlayers)
    {
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

        tooManyEvilPlayers = evilRolesCount <= evilPlayersTargetCount;

        roles.AddRange(Enumerable.Repeat(Role.EvilEntity, evilPlayersTargetCount - evilRolesCount));
        roles.AddRange(Enumerable.Repeat(Role.GoodKnight, goodPlayersTargetCount - goodRolesCount));

        roles.Shuffle();
        return roles;
    }
    public static Squad MakeFirstSquad(Player leader, int playersCount)
    {
        var firstSquadSize = GameCountsCalculator.GetSquadRequiredSize(playersCount, 1);
        var firstSquad = new Squad()
        {
            Leader = leader,
            QuestNumber = 1,
            RoundNumber = 1,
            RequiredPlayersNumber = firstSquadSize,
            Status = SquadStatus.SquadVoting,
        };
        return firstSquad;
    }
}