using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Incoming;
using servartur.Models.Outgoing;
using System.Reflection.Metadata;

namespace servartur.Services;

public interface IVoteService
{
    SquadInfoDto GetUpdatedCurrentSquad(int roomId);
    List<QuestInfoShortDto> GetUpdatedQuestsSummary(int roomId);
    void VoteSquad(CastVoteDto voteDto, out bool votingEnded, out int roomId);
    void VoteQuest(CastVoteDto voteDto, out bool votingEnded, out int roomId);

}
public class VoteService : DataUpdatesService, IVoteService
{
    public VoteService(GameDbContext dbContext, IMapper mapper, ILogger<VoteService> logger)
        : base(dbContext, mapper, logger) { }


    public void VoteSquad(CastVoteDto voteDto, out bool votingEnded, out int roomId)
    {
        var squad = _dbContext.Squads
            .Include(s => s.SquadVotes)
            .Include(s => s.Room)
                .ThenInclude(r => r.Players)
            .FirstOrDefault(s => s.SquadId == voteDto.SquadId)
            ?? throw new SquadNotFoundException(voteDto.SquadId);
        if (squad.Status != SquadStatus.Submitted)
            throw new SquadInWrongStateException(voteDto.SquadId);

        if (squad.SquadVotes.Any(v => v.VoterId == voteDto.VoterId))
        {
            roomId = squad.RoomId;
            votingEnded = false;
            return;
        }

        var vote = _mapper.Map<SquadVote>(voteDto);
        squad.SquadVotes.Add(vote);
        _dbContext.SaveChanges();
        _dbContext.Entry(squad).Reload();

        recountSquadVotes(squad, out votingEnded);
        roomId = squad.RoomId;
    }
    private void recountSquadVotes(Squad squad, out bool votingEnded)
    {
        int playerCount = squad.Room.Players.Count; // TODO add this as a squad field
        votingEnded = playerCount == squad.SquadVotes.Count;
        if (!votingEnded) return;

        int positiveVotesCount = squad.SquadVotes.Where(v => v.Value == true).Count();
        if (positiveVotesCount > playerCount / 2)
        {
            squad.Status = SquadStatus.Approved;
            _dbContext.SaveChanges();
        }
        else
        {
            squad.Status = SquadStatus.Rejected;
            _dbContext.SaveChanges();

            handleSquadRejection(squad.RoomId);
        }
    }
    private void handleSquadRejection(int roomId)
    {
        Squad nextSquad = makeNextSquad(roomId, out Room room);

        if (nextSquad.PrevRejectionCount < GameCountsCalculator.MaxNumberOfPrevRejection)
        {
            _dbContext.Squads.Add(nextSquad);
            room.CurrentSquad = nextSquad;
            _dbContext.SaveChanges();
        }
        else
        {
            winEvil(roomId);
        }

    }

    public void VoteQuest(CastVoteDto voteDto, out bool votingEnded, out int roomId)
    {
        var squad = _dbContext.Squads
           .Include(s => s.QuestVotes)
           .Include(s => s.Room)
               .ThenInclude(r => r.Players)
           .FirstOrDefault(s => s.SquadId == voteDto.SquadId)
           ?? throw new SquadNotFoundException(voteDto.SquadId);
        if (squad.Status != SquadStatus.Submitted)
            throw new SquadInWrongStateException(voteDto.SquadId);

        if (squad.QuestVotes.Any(v => v.VoterId == voteDto.VoterId))
        {
            roomId = squad.RoomId;
            votingEnded = false;
            return;
        }

        var vote = _mapper.Map<QuestVote>(voteDto);
        squad.QuestVotes.Add(vote);
        _dbContext.SaveChanges();
        _dbContext.Entry(squad).Reload();

        recountQuestVotes(squad, out votingEnded);
        roomId = squad.RoomId;
    }
    private void recountQuestVotes(Squad squad, out bool votingEnded)
    {
        votingEnded = squad.RequiredMembersNumber == squad.QuestVotes.Count;
        if (!votingEnded) return;

        int negativeVotesCount = squad.QuestVotes.Where(v => v.Value == false).Count();
        if (negativeVotesCount < (squad.IsDoubleFail ? 2 : 1))
        {
            squad.Status = SquadStatus.Successful;
        }
        else
        {
            squad.Status = SquadStatus.Failed;
        }
        _dbContext.SaveChanges();
        handleQuestFinished(squad.RoomId);
    }
    private void handleQuestFinished(int roomId)
    {
        var squads = _dbContext.Squads.Where(r => r.RoomId == roomId);

        // TODO move this logic to DomainLogic (?)
        int successfulQuestsCount = squads.Where(s => s.Status == SquadStatus.Successful).Count();
        int failedQuestsCount = squads.Where(s => s.Status == SquadStatus.Failed).Count();

        if (successfulQuestsCount > 3)
        {
            winGood(roomId);
        }
        else if (successfulQuestsCount > 3)
        {
            winEvil(roomId);
        }
        else
        {
            var nextSquad = makeNextSquad(roomId, out Room room);

            _dbContext.Squads.Add(nextSquad);
            room.CurrentSquad = nextSquad;
            _dbContext.SaveChanges();
        }

    }

    private Squad makeNextSquad(int roomId, out Room room)
    {
        room = _dbContext.Rooms
             .Include(r => r.Players)
             .Include(r => r.Squads)
             .Include(r => r.CurrentSquad)
             .FirstOrDefault(r => r.RoomId == roomId)
             ?? throw new RoomNotFoundException(roomId);

        Squad prevSquad = room.CurrentSquad
            ?? throw new SquadNotFoundException(roomId);

        int leaderIndex = room.Players.IndexOf(prevSquad.Leader);
        int nextLeaderIndex = (leaderIndex + 1) % room.Players.Count;
        var nextLeader = room.Players[nextLeaderIndex];
        Squad nextSquad = new Squad()
        {
            QuestNumber = prevSquad.QuestNumber,
            SquadNumber = prevSquad.SquadNumber + 1,
            RequiredMembersNumber = prevSquad.RequiredMembersNumber,
            IsDoubleFail = prevSquad.IsDoubleFail,
            Status = SquadStatus.SquadChoice,
            Leader = nextLeader,
        };

        return nextSquad;

    }
    private void winEvil(int roomId)
    {
        var room = _dbContext.Rooms
             .FirstOrDefault(r => r.RoomId == roomId)
             ?? throw new RoomNotFoundException(roomId);

        room.Status = RoomStatus.Result;
        _dbContext.SaveChanges();
        // TODO UPDATE room INFO
    }
    private void winGood(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        if (room.Players.Any(p => p.Role == Role.Assassin))
            room.Status = RoomStatus.Assassination;
        else
            room.Status = RoomStatus.Result;

        _dbContext.SaveChanges();
        // TODO UPDATE Room INFO
    }
}
