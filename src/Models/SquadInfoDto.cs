using servartur.Entities;
using servartur.Enums;

namespace servartur.Models;

public class SquadInfoDto
{
    public int SquadId { get; set; }
    public int QuestNumber { get; set; }
    public int RoundNumber { get; set; }
    public int RequiredPlayersNumber { get; set; }
    public SquadStatus Status { get; set; }
    public int LeaderId { get; set; }

    public List<PlayerInfoDto> MembersPlayerIds { get; set; } = null!;
}
