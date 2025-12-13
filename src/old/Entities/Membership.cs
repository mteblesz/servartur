namespace servartur.Entities;

internal class Membership
{
    public int SquadId { get; set; }
    public virtual required Squad Squad { get; set; }

    public int PlayerId { get; set; }
    public virtual required Player Player { get; set; }
}
