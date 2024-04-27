using servartur.Enums;

namespace servartur.Models.Outgoing;

public class QuestInfoDto
{
    public required int SquadId { get; set; }
    public required int QuestNumber { get; set; }
    public required int SquadNumber { get; set; }
    public required int RequiredPlayersNumber { get; set; }
    public required SquadStatus Status { get; set; }
    public required PlayerInfoDto Leader { get; set; }

    public required List<PlayerInfoDto> Members { get; set; }
    public required List<VoteInfoDto> SquadVoteInfo { get; set; }
    public required int? QuestVoteSuccessCount { get; set; }
}
