using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models;

namespace servartur.Services;

public interface IInfoService
{
    RoomInfoDto GetRoomById(int roomId);
    PlayerInfoDto GetPlayerById(int playerId);
    List<PlayerInfoDto> GetFilteredPlayers(int roomId, Predicate<Player> predicate);
    SquadInfoDto GetQuestBySquadId(int squadId);
}
public class InfoService : IInfoService
{
    private readonly GameDbContext _dbContext;
    private readonly IMapper _mapper;
    public readonly ILogger<MatchupService> _logger;

    public InfoService(GameDbContext dbContext, IMapper mapper, ILogger<MatchupService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public RoomInfoDto GetRoomById(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var result = _mapper.Map<RoomInfoDto>(room);
        return result;
    }

    public PlayerInfoDto GetPlayerById(int playerId)
    {
        throw new NotImplementedException();
    }

    //get good, evil etc, define predicate in controller
    public List<PlayerInfoDto> GetFilteredPlayers(int roomId, Predicate<Player> predicate)
    {
        throw new NotImplementedException();
    }

    public SquadInfoDto GetQuestBySquadId(int squadId)
    {
        throw new NotImplementedException();
    }
}
