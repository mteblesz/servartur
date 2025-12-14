namespace servartur.Entities;

#pragma warning disable CA1515 // Consider making public types internal
public class Membership
{
    public int SquadId { get; set; }
    public virtual required Squad Squad { get; set; }

    public int PlayerId { get; set; }
    public virtual required Player Player { get; set; }
}
