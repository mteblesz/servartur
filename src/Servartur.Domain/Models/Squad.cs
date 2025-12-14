using Servartur.Domain.Models.Enums;

namespace Servartur.Domain.Models;

public class Squad
{
    public Guid Id { get; set; }

    public QuestStatus Status { get; set; }

    public uint QuestNumber { get; set; }

    public uint SquadNumber { get; set; }

    public int RequiredMembersNumber { get; set; }

    public bool IsDoubleFail { get; set; }
}
