using servartur.Enums;

namespace servartur.Models.Outgoing;

public class QuestInfoDto
{
    public int SquadId { get; set; }
    public int QuestNumber { get; set; }
    public int SquadNumber { get; set; }
    public int RequiredPlayersNumber { get; set; }
    public SquadStatus Status { get; set; }
    public PlayerInfoDto Leader { get; set; } = null!;

    public List<PlayerInfoDto> Members { get; set; } = null!;
    public List<VoteInfoDto> SquadVoteInfo { get; set; } = null!;
    public int? QuestVoteSuccessCount { get; set; }
}
