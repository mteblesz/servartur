using servartur.Entities;
using servartur.Enums;

namespace servartur.Models.Outgoing;

public class SquadInfoDto
{
    public int SquadId { get; set; }
    public int QuestNumber { get; set; }
    public int RoundNumber { get; set; }
    public int RequiredPlayersNumber { get; set; }
    public SquadStatus Status { get; set; }
    public PlayerInfoDto Leader { get; set; } = null!;

    public List<PlayerInfoDto> Members { get; set; } = null!;
    public List<SquadVoteInfoDto> SquadVoteInfo { get; set; } = null!;
    public int? QuestVoteSuccessCount { get; set; }
}

// public int IsDoubleFailSquad TODO!!!
// dodac pole, przy tworzeniu nowego squadu
// sprawdzac w domainlogic ilosc graczy plu snr squadu,
// ilosc graczy brac z room
