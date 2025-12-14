using Servartur.Domain.Models.Enums;

namespace Servartur.Domain.Models;

public class Squad
{
    public Guid Id { get; set; }

    public required QuestStatus Status { get; set; }

    public required int QuestNumber { get; set; }

    public required int SquadNumber { get; set; }

    public required int RequiredMembersNumber { get; set; }

    public required bool IsDoubleFail { get; set; }
}
