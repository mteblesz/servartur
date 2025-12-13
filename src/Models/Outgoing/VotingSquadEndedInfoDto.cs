namespace servartur.Models.Outgoing;

public class VotingSquadEndedInfoDto
{
    public required SquadInfoDto CurrentSquadInfo { get; set; }
    public required EndGameInfoDto EndGameInfo { get; set; }
}