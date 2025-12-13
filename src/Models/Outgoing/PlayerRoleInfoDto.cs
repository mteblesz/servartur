namespace servartur.Models.Outgoing;

public class PlayerRoleInfoDto
{
    public required int PlayerId { get; set; }
    public required string Team { get; set; }
    public required string Role { get; set; }
}
