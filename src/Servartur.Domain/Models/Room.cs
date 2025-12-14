using Servartur.Domain.Models.Enums;

namespace Servartur.Domain.Models;

public class Room
{
    public Guid Id { get; set; }

    public RoomStatus Status { get; set; }
}
