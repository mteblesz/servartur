﻿using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Enums;
using System.Xml.Schema;

namespace servartur.Seeders;

public class DbSeeder
{
    private readonly GameDbContext dbContext;

    public DbSeeder(GameDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Seed()
    {
        if (dbContext.Database.CanConnect())
        {
            if (!dbContext.Rooms.Any())
            {
                var partialRooms = new List<Room>() { new Room(), new Room() };
                dbContext.Rooms.AddRange(partialRooms);
                dbContext.SaveChanges();

                var players = getPlayers(partialRooms);
                dbContext.Players.AddRange(players);
                dbContext.SaveChanges();

                var squads = getSquads(partialRooms, players);
                dbContext.Squads.AddRange(squads);
                dbContext.SaveChanges();

                fillRooms(partialRooms, players, squads);
                dbContext.SaveChanges();



                var memberships = getMemberships(players, squads);
                dbContext.Memberships.AddRange(memberships);
                dbContext.SaveChanges();

                var squadVotes = getSquadVotes(players, squads);
                dbContext.SquadVotes.AddRange(squadVotes);
                dbContext.SaveChanges();

                var questVotes = getQuestVotes(players, squads);
                dbContext.QuestVotes.AddRange(questVotes);
                dbContext.SaveChanges();
            }
        }
    }

    private List<Player> getPlayers(List<Room> rooms)
    {
        return
                [
                    new Player()
                    {
                        Nick = "michal",
                        Team = Team.Evil,
                        Role = Role.Assassin,
                        RoomId = rooms[0].RoomId,
                    },
                    new Player()
                    {
                        Nick = "kamil",
                        Team = Team.Good,
                        Role = Role.Merlin,
                        RoomId = rooms[0].RoomId,
                    },
                    new Player()
                    {
                        Nick = "janek",
                        Team = Team.Evil,
                        Role = Role.Morgana,
                        RoomId = rooms[0].RoomId,
                    },
                    new Player()
                    {
                        Nick = "jedrek",
                        Team = Team.Good,
                        Role = Role.Percival,
                        RoomId = rooms[0].RoomId,
                    },
                    new Player()
                    {
                        Nick = "szymon",
                        Team = Team.Good,
                        Role = null,
                        RoomId = rooms[0].RoomId,
                    },

                    new Player()
                    {
                        Nick = "michal_1",
                        Team = Team.Good,
                        Role = Role.Merlin,
                        RoomId = rooms[1].RoomId,
                    },
                    new Player()
                    {
                        Nick = "kamil_1",
                        Team = Team.Evil,
                        Role = Role.Assassin,
                        RoomId = rooms[1].RoomId,
                    },
                    new Player()
                    {
                        Nick = "janek_1",
                        Team = Team.Good,
                        Role = Role.Percival,
                        RoomId = rooms[1].RoomId,
                    },
                    new Player()
                    {
                        Nick = "jedrek_1",
                        Team = Team.Good,
                        Role = null,
                        RoomId = rooms[1].RoomId,
                    },
                    new Player()
                    {
                        Nick = "szymon_1",
                        Team = Team.Evil,
                        Role = Role.Morgana,
                        RoomId = rooms[1].RoomId,
                    },
                ];
    }
    private List<Squad> getSquads(List<Room> rooms, List<Player> players)
    {
        return
                [
                    new Squad()
                    {
                        QuestNumber = 1,
                        RoundNumber = 1,
                        RequiredPlayersNumber = 2,
                        Status = SquadStatus.Rejected,
                        Leader = players[0],
                        RoomId = rooms[0].RoomId,
                    },
                    new Squad()
                    {
                        QuestNumber = 1,
                        RoundNumber = 2,
                        RequiredPlayersNumber = 2,
                        Status = SquadStatus.Successfull,
                        Leader = players[1],
                        RoomId = rooms[0].RoomId,
                    },
                    new Squad()
                    {
                        QuestNumber = 2,
                        RoundNumber = 3,
                        RequiredPlayersNumber = 3,
                        Status = SquadStatus.Failed,
                        Leader = players[2],
                        RoomId = rooms[0].RoomId,
                    },
                    new Squad()
                    {
                        QuestNumber = 3,
                        RoundNumber = 4,
                        RequiredPlayersNumber = 2,
                        Status = SquadStatus.Failed,
                        Leader = players[3],
                        RoomId = rooms[0].RoomId,
                    },
                    new Squad()
                    {
                        QuestNumber = 4,
                        RoundNumber = 5,
                        RequiredPlayersNumber = 2,
                        Status = SquadStatus.Successfull,
                        Leader = players[4],
                        RoomId = rooms[0].RoomId,
                    },
                    new Squad()
                    {
                        QuestNumber = 5,
                        RoundNumber = 6,
                        RequiredPlayersNumber = 3,
                        Status = SquadStatus.Successfull,
                        Leader = players[0],
                        RoomId = rooms[0].RoomId,
                    },
                ];
    }

    private static List<Room> fillRooms(List<Room> rooms, List<Player> players, List<Squad> squads)
    {
        // add player and squad info
        foreach (var room in rooms)
        {
            var roomPlayers = players.Where(p => p.RoomId == room.RoomId).ToList();
            room.Players.AddRange(roomPlayers);
            var roomSquads = squads.Where(s => s.RoomId == room.RoomId).ToList();
            room.Squads.AddRange(roomSquads);
        }
        //add other info
        if (rooms.Count == 2)
        {
            rooms[0].Status = RoomStatus.Result;
            rooms[0].LastRoundWithSquadOnQuest = 5;
            rooms[0].CurrentSquad = squads[4];
            rooms[0].CurrentSquadId = squads[4].SquadId;
            rooms[0].Assassination = new Assassination() { Assassin = players[0], Target = players[1], Result = true };

            rooms[1].Status = RoomStatus.Matchup;
            rooms[1].LastRoundWithSquadOnQuest = -1;
            rooms[1].CurrentSquad = null;
        }
        return rooms;
    }

    private List<Membership> getMemberships(List<Player> players, List<Squad> squads)
    {
        return
        [
            new Membership() { Squad = squads[0], Player = players[0] },
            new Membership() { Squad = squads[0], Player = players[1] },

            new Membership() { Squad = squads[1], Player = players[1] },
            new Membership() { Squad = squads[1], Player = players[3] },

            new Membership() { Squad = squads[2], Player = players[2] },
            new Membership() { Squad = squads[2], Player = players[3] },
            new Membership() { Squad = squads[2], Player = players[1] },

            new Membership() { Squad = squads[3], Player = players[3] },
            new Membership() { Squad = squads[3], Player = players[2] },

            new Membership() { Squad = squads[4], Player = players[4] },
            new Membership() { Squad = squads[4], Player = players[3] },

            new Membership() { Squad = squads[5], Player = players[0] },
            new Membership() { Squad = squads[5], Player = players[3] },
            new Membership() { Squad = squads[5], Player = players[1] },
        ];
    }

    private List<QuestVote> getQuestVotes(List<Player> players, List<Squad> squads)
    {
        return
        [
            new QuestVote() { Squad = squads[1], Player = players[1], Value = true },
            new QuestVote() { Squad = squads[1], Player = players[3], Value = true },

            new QuestVote() { Squad = squads[2], Player = players[2], Value = true },
            new QuestVote() { Squad = squads[2], Player = players[3], Value = true },
            new QuestVote() { Squad = squads[2], Player = players[1], Value = false },

            new QuestVote() { Squad = squads[3], Player = players[3], Value = true },
            new QuestVote() { Squad = squads[3], Player = players[2], Value = false },

            new QuestVote() { Squad = squads[4], Player = players[4], Value = true },
            new QuestVote() { Squad = squads[4], Player = players[3], Value = true },

            new QuestVote() { Squad = squads[5], Player = players[4], Value = true },
            new QuestVote() { Squad = squads[5], Player = players[3], Value = true },
            new QuestVote() { Squad = squads[5], Player = players[1], Value = true },
        ];
    }

    private List<SquadVote> getSquadVotes(List<Player> players, List<Squad> squads)
    {
        return
        [
            new SquadVote() { Squad = squads[0], Player = players[0], Value = true },
            new SquadVote() { Squad = squads[0], Player = players[1], Value = true },
            new SquadVote() { Squad = squads[0], Player = players[2], Value = false },
            new SquadVote() { Squad = squads[0], Player = players[3], Value = false },
            new SquadVote() { Squad = squads[0], Player = players[4], Value = false },

            new SquadVote() { Squad = squads[1], Player = players[0], Value = false },
            new SquadVote() { Squad = squads[1], Player = players[1], Value = true },
            new SquadVote() { Squad = squads[1], Player = players[2], Value = true },
            new SquadVote() { Squad = squads[1], Player = players[3], Value = true },
            new SquadVote() { Squad = squads[1], Player = players[4], Value = true },

            new SquadVote() { Squad = squads[2], Player = players[0], Value = false },
            new SquadVote() { Squad = squads[2], Player = players[1], Value = true },
            new SquadVote() { Squad = squads[2], Player = players[2], Value = true },
            new SquadVote() { Squad = squads[2], Player = players[3], Value = true },
            new SquadVote() { Squad = squads[2], Player = players[4], Value = false },

            new SquadVote() { Squad = squads[3], Player = players[0], Value = true },
            new SquadVote() { Squad = squads[3], Player = players[1], Value = true },
            new SquadVote() { Squad = squads[3], Player = players[2], Value = false },
            new SquadVote() { Squad = squads[3], Player = players[3], Value = true },
            new SquadVote() { Squad = squads[3], Player = players[4], Value = false },

            new SquadVote() { Squad = squads[4], Player = players[0], Value = false },
            new SquadVote() { Squad = squads[4], Player = players[1], Value = true },
            new SquadVote() { Squad = squads[4], Player = players[2], Value = false },
            new SquadVote() { Squad = squads[4], Player = players[3], Value = true },
            new SquadVote() { Squad = squads[4], Player = players[4], Value = true },

            new SquadVote() { Squad = squads[5], Player = players[0], Value = false },
            new SquadVote() { Squad = squads[5], Player = players[1], Value = true },
            new SquadVote() { Squad = squads[5], Player = players[2], Value = false },
            new SquadVote() { Squad = squads[5], Player = players[3], Value = true },
            new SquadVote() { Squad = squads[5], Player = players[4], Value = true },

        ];
    }

}
