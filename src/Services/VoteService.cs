using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Incoming;
using servartur.Models.Outgoing;
using servartur.Services.Base_Services;

namespace servartur.Services;

public interface IVoteService
{
    public VotingSquadEndedInfoDto GetUpdatedSquadVotingEnded(int roomId);
    public VotingQuestEndedInfoDto GetUpdatedQuestVotingEnded(int roomId);
    void VoteSquad(CastVoteDto voteDto, out bool votingEnded, out int roomId);
    void VoteQuest(CastVoteDto voteDto, out bool votingEnded, out int roomId);

}
public class VoteService : VotingUpdates, IVoteService
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
            throw new PlayerHasAlreadyVotedException(voteDto.SquadId);

        var vote = _mapper.Map<SquadVote>(voteDto);
        squad.SquadVotes.Add(vote);
        _dbContext.SaveChanges();
        _dbContext.Entry(squad).Reload();

        recountSquadVotes(squad, out votingEnded);

        roomId = squad.RoomId;
    }
    private void recountSquadVotes(Squad squad, out bool votingEnded)
    {
        votingEnded = true;
        var votingResult = ResultsCalculator.CountSquadVotes(squad.Room.Players.Count, squad.SquadVotes);
        switch (votingResult)
        {
            case ResultsCalculator.SquadVotingResult.Unfinished:
                votingEnded = false;
                break;

            case ResultsCalculator.SquadVotingResult.Approved:
                squad.Status = SquadStatus.Approved;
                _dbContext.SaveChanges();
                break;

            case ResultsCalculator.SquadVotingResult.Rejected:
                squad.Status = SquadStatus.Rejected;
                _dbContext.SaveChanges();

                handleSquadRejection(squad.RoomId);
                break;
        }
    }
    private void handleSquadRejection(int roomId)
    {
        var room = _dbContext.Rooms
              .Include(r => r.Players)
              .Include(r => r.Squads)
              .Include(r => r.CurrentSquad)
              .FirstOrDefault(r => r.RoomId == roomId)
              ?? throw new RoomNotFoundException(roomId);

        var prevSquad = room.CurrentSquad
            ?? throw new SquadNotFoundException(roomId);

        var nextSquad = SquadFactory.OnRejection(room.Players, prevSquad, prevSquad.Leader);

        if (nextSquad.PrevRejectionCount < GameCountsCalculator.MaxNumberOfPrevRejection)
        {
            room.Squads.Add(nextSquad);
            room.CurrentSquad = nextSquad;
            _dbContext.SaveChanges();
        }
        else
        {
            endGame(roomId, false);
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
        if (squad.Status != SquadStatus.Approved)
            throw new SquadInWrongStateException(voteDto.SquadId);

        if (squad.QuestVotes.Any(v => v.VoterId == voteDto.VoterId))
            throw new PlayerHasAlreadyVotedException(voteDto.SquadId);

        var vote = _mapper.Map<QuestVote>(voteDto);
        squad.QuestVotes.Add(vote);
        _dbContext.SaveChanges();
        _dbContext.Entry(squad).Reload();

        recountQuestVotes(squad, out votingEnded);
        roomId = squad.RoomId;
    }
    private void recountQuestVotes(Squad squad, out bool votingEnded)
    {
        votingEnded = true;
        var votingResult = ResultsCalculator.CountQuestVotes(squad.RequiredMembersNumber, squad.QuestVotes, squad.IsDoubleFail);
        switch (votingResult)
        {
            case ResultsCalculator.QuestVotingResult.Unfinished:
                votingEnded = false;
                break;

            case ResultsCalculator.QuestVotingResult.Successful:
                squad.Status = SquadStatus.Successful;
                _dbContext.SaveChanges();

                handleQuestFinished(squad.RoomId);
                break;

            case ResultsCalculator.QuestVotingResult.Failed:
                squad.Status = SquadStatus.Failed;
                _dbContext.SaveChanges();

                handleQuestFinished(squad.RoomId);
                break;
        }
    }
    private void handleQuestFinished(int roomId)
    {
        var squads = _dbContext.Squads.Where(r => r.RoomId == roomId).ToList();
        var gameResult = ResultsCalculator.EvaluateGame(squads);

        if (gameResult == ResultsCalculator.GameResult.Unfinished)
        {
            var room = _dbContext.Rooms
                 .Include(r => r.Players)
                 .Include(r => r.Squads)
                 .Include(r => r.CurrentSquad)
                 .FirstOrDefault(r => r.RoomId == roomId)
                 ?? throw new RoomNotFoundException(roomId);

            Squad prevSquad = room.CurrentSquad
                ?? throw new SquadNotFoundException(roomId);

            Squad nextSquad = SquadFactory.OnQuestFinished(room.Players, prevSquad, prevSquad.Leader);

            room.Squads.Add(nextSquad);
            room.CurrentSquad = nextSquad;
            _dbContext.SaveChanges();
        }
        else
        {
            endGame(roomId, gameResult == ResultsCalculator.GameResult.GoodWin);
        }
    }

    private void endGame(int roomId, bool goodPlayersWon)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        if (goodPlayersWon == true)
        {
            if (room.Players.Any(p => p.Role == Role.Assassin))
                room.Status = RoomStatus.Assassination;
            else
                room.Status = RoomStatus.ResultGoodWin;
        }
        else
        {
            room.Status = RoomStatus.ResultEvilWin;
        }

        _dbContext.SaveChanges();
    }
}
