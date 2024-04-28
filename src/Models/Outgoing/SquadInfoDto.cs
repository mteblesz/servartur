﻿using servartur.Entities;
using servartur.Enums;

namespace servartur.Models.Outgoing;

public class SquadInfoDto
{
    public required int SquadId { get; set; }
    public required int QuestNumber { get; set; }
    public required int PrevRejectionCount { get; set; }
    public required int RequiredMembersNumber { get; set; }
    public required SquadStatus Status { get; set; }
    public required PlayerInfoDto Leader { get; set; }

    public required List<PlayerInfoDto> Members { get; set; }
}