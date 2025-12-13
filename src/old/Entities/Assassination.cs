namespace servartur.Entities;

internal class Assassination
{
    public int AssassinationId { get; set; }
    public bool Result { get; set; }

    public int AssassinId { get; set; }
    public virtual Player Assassin { get; set; } = null!;

    public int TargetId { get; set; }
    public virtual Player Target { get; set; } = null!;

    public int RoomId { get; set; } // Required foreign key property
}
