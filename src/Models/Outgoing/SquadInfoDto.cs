using servartur.Entities;
using servartur.Enums;

namespace servartur.Models.Outgoing;

public class SquadInfoDto
{
    public int SquadId { get; set; }
    public int QuestNumber { get; set; }
    public int RejectionsLeftToEvilWin { get; set; }
    public int RequiredPlayersNumber { get; set; }
    public SquadStatus Status { get; set; }
    public PlayerInfoDto Leader { get; set; } = null!;

    public List<PlayerInfoDto> Members { get; set; } = null!;
}