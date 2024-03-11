using servartur.Enums;

namespace servartur.Models.Outgoing;

public class QuestInfoShortDto
{
    public int SquadId { get; set; }
    public int QuestNumber { get; set; }
    public int RequiredPlayersNumber { get; set; }
    public int IsDoubleFailSquad { get; set; }
    public SquadStatus Status { get; set; }
}
