namespace servartur.Models;

public class PlayerInfoDto
{
    public int PlayerId { get; set; }
    public string Nick { get; set; } = null!;
    public string Team { get; set; } = null!;
    public string Role { get; set; } = null!;
}
