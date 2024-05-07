using servartur.Enums;

namespace servartur.Entities;
public class Squad
{
    public Squad()
    {
        Status = SquadStatus.Upcoming;
    }
    public int SquadId { get; set; }
    /// <summary>
    /// number of this squad's quest
    /// </summary>
    public int QuestNumber { get; set; }
    /// <summary>
    /// Number assigned to (this) squad for it's quest. Starts from 1
    /// </summary>
    public int SquadNumber { get; set; }
    /// <summary>
    /// number of squads that failed Squad Voting for this quest
    /// </summary>
    public int PrevRejectionCount => SquadNumber - 1;
    public int RequiredMembersNumber { get; set; }
    public bool IsDoubleFail { get; set; }
    public SquadStatus Status { get; set; }

    public int LeaderId { get; set; }
    public virtual Player Leader { get; set; } = null!;

    public int RoomId { get; set; }
    public virtual Room Room { get; set; } = null!;



    public virtual List<Membership> Memberships { get; set; } = [];
    public virtual List<SquadVote> SquadVotes { get; set; } = [];
    public virtual List<QuestVote> QuestVotes { get; set; } = [];


}


