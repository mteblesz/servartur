using servartur.Enums;

namespace servartur.Models.Outgoing;

internal class QuestInfoDto
{
    public required int SquadId { get; set; }
    public required int QuestNumber { get; set; }
    public required int SquadNumber { get; set; }
    public required int RequiredMembersNumber { get; set; }
    public required SquadStatus Status { get; set; }
    public required PlayerInfoDto Leader { get; set; }

#pragma warning disable CA1002 // Do not expose generic lists
    public required List<PlayerInfoDto> Members { get; set; }
#pragma warning restore CA1002 // Do not expose generic lists
#pragma warning disable CA1002 // Do not expose generic lists
    public required List<VoteInfoDto> SquadVoteInfo { get; set; }
#pragma warning restore CA1002 // Do not expose generic lists
    public required int? QuestVoteSuccessCount { get; set; }
}
