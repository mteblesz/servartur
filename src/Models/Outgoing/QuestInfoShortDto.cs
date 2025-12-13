using servartur.Enums;

namespace servartur.Models.Outgoing;

public class QuestInfoShortDto
{
    public required int? SquadId { get; set; }
    public required int QuestNumber { get; set; }
    public required int RequiredMembersNumber { get; set; }
    public required bool IsDoubleFail { get; set; }
    public required SquadStatus Status { get; set; }
}
