using servartur.Enums;

namespace servartur.Entities;
public class Room
{
    public Room()
    {
        Status = RoomStatus.Unknown;
        LastRoundWithSquadOnQuest = -1;
    }

    public int RoomId { get; set; }
    public RoomStatus Status { get; set; }
    public int LastRoundWithSquadOnQuest { get; set; }

    public int? CurrentSquadId { get; set; }
    public virtual Squad? CurrentSquad { get; set; }
    public virtual Assassination? Assassination { get; set; }

    public virtual List<Player> Players { get; set; } = [];
    public virtual List<Squad> Squads { get; set; } = [];

}

