using Servartur.Domain.Models.Enums;

namespace Servartur.Domain.Models;

public class Player
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Character? Character { get; set; }
}
