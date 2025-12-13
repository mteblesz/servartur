namespace servartur.Entities;

public class Membership
{
    public int SquadId { get; set; }
    public required virtual Squad Squad { get; set; }

    public int PlayerId { get; set; }
    public required virtual Player Player { get; set; }
}
