namespace servartur.Models.Outgoing;

internal class VotingQuestEndedInfoDto
{
    public required SquadInfoDto CurrentSquadInfo { get; set; }
#pragma warning disable CA1002 // Do not expose generic lists
    public required List<QuestInfoShortDto> QuestsSummary { get; set; }
#pragma warning restore CA1002 // Do not expose generic lists
    public required EndGameInfoDto EndGameInfo { get; set; }
}
