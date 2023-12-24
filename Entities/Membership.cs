﻿namespace servartur.Entities;

public class Membership
{
    public int SquadId { get; set; }
    public virtual Squad Squad { get; set; }

    public int PlayerId { get; set; }
    public virtual Player Player { get; set; }
}
