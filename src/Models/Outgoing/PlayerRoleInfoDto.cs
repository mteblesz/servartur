namespace servartur.Models.Outgoing;

public class PlayerRoleInfoDto
{
    public int PlayerId { get; set; }
    public string Team { get; set; } = null!;
    public string Role { get; set; } = null!;
}
