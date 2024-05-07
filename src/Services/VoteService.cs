using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Incoming;
using servartur.Models.Outgoing;

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
        int playerCount = squad.Room.Players.Count;
        votingEnded = playerCount == squad.SquadVotes.Count;
        if (!votingEnded) return;

        var positiveVotesCount = squad.SquadVotes.Where(v => v.Value == true).Count();
        if (positiveVotesCount > playerCount / 2)
        {
            squad.Status = SquadStatus.Approved;
            _dbContext.SaveChanges();
        }
        else
        {
            squad.Status = SquadStatus.Rejected;
            _dbContext.SaveChanges();

            handleSquadRejection(squad);
        }
    }
    private void handleSquadRejection(Squad squad)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .Include(r => r.Squads)
            .FirstOrDefault(r => r.RoomId == squad.RoomId)
            ?? throw new RoomNotFoundException(squad.RoomId);

        int leaderIndex = room.Players.IndexOf(squad.Leader);
        int nextLeaderIndex = (leaderIndex + 1) % room.Players.Count;
        var nextLeader = room.Players[nextLeaderIndex];
        Squad nextSquad = new Squad()
        {
            QuestNumber = squad.QuestNumber,
            SquadNumber = squad.SquadNumber + 1,
            RequiredMembersNumber = squad.RequiredMembersNumber,
            IsDoubleFail = squad.IsDoubleFail,
            Status = SquadStatus.SquadChoice,
            Leader = nextLeader,
        };

        if (nextSquad.PrevRejectionCount < GameCountsCalculator.MaxNumberOfPrevRejection)
        {
            room.Squads.Add(nextSquad);
            room.CurrentSquad = nextSquad;
            _dbContext.SaveChanges();
        }
        else
        {
            winEvil(room);
        }

    }

    public void VoteQuest(CastVoteDto voteDto, out bool votingEnded, out int roomId)
    {
        throw new NotImplementedException();
    }
    private void nextQuest(int roomId)
    {
        throw new NotImplementedException();
    }
    private void winEvil(Room room)
    {
        throw new NotImplementedException();
    }
    private void winGood(Room room)
    {
        throw new NotImplementedException();
    }
}
