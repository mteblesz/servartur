namespace servartur.Models.Outgoing;

public class VotingQuestEndedInfoDto
{
    public required SquadInfoDto CurrentSquadInfo { get; set; }
    public required List<QuestInfoShortDto> QuestsSummary { get; set; }
    public required EndGameInfoDto EndGameInfo { get; set; }
}