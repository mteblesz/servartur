using servartur.Enums;

namespace servartur.Entities;


#pragma warning disable CA1515 // Consider making public types internal
public class Room
{
    public Room()
    {
        Status = RoomStatus.Unknown;
    }

    public int RoomId { get; set; }
    public RoomStatus Status { get; set; }

    public int? CurrentSquadId { get; set; }
    public virtual Squad? CurrentSquad { get; set; }
    public virtual Assassination? Assassination { get; set; }

#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
    public virtual List<Player> Players { get; set; } = [];
    public virtual List<Squad> Squads { get; set; } = [];

}

