﻿using AutoMapper;
using servartur.Entities;

namespace servartur.Services;

public interface ISquadService
{
    void AddMember(int playerId);
    void RemoveMember(int playerId);
    void SubmitSquad(int squadId);
}
public class SquadService : DataUpdatesService, ISquadService
{
    public SquadService(GameDbContext dbContext, IMapper mapper, ILogger<SquadService> logger)
        : base(dbContext, mapper, logger) { }

    public void AddMember(int playerId)
    {
        throw new NotImplementedException();
    }

    public void RemoveMember(int playerId)
    {
        throw new NotImplementedException();
    }

    public void SubmitSquad(int squadId)
    {
        throw new NotImplementedException();
    }
}
