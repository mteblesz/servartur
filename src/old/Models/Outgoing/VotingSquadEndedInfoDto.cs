namespace servartur.Models.Outgoing;

internal class VotingSquadEndedInfoDto
{
    public required SquadInfoDto CurrentSquadInfo { get; set; }
    public required EndGameInfoDto EndGameInfo { get; set; }
}
