using servartur.Enums;
using System.ComponentModel.DataAnnotations;

namespace servartur.Entities;
public class Player
{
    public int PlayerId { get; set; }

    [StringLength(maximumLength: 20, MinimumLength = 3)]
    public string? Nick { get; set; }

    public Team? Team { get; set; }

    public Role? Role { get; set; }

    public int RoomId { get; set; } 
    public virtual Room Room { get; set; }

    public virtual List<Membership> Memberships { get; set; } = [];
    public virtual List<SquadVote> SquadVotes { get; set; } = [];
    public virtual List<QuestVote> QuestVotes { get; set; } = [];
}

