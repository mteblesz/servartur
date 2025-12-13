using AutoMapper;
using servartur.Entities;

namespace servartur.Services;

/// <summary>
/// Stores common fields all services use.
/// </summary>
internal abstract class BaseService
{
    protected readonly GameDbContext _dbContext;
    protected readonly IMapper _mapper;
    protected readonly ILogger _logger;

    protected BaseService(GameDbContext dbContext, IMapper mapper, ILogger logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }
}
