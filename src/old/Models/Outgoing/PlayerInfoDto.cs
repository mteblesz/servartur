namespace servartur.Models.Outgoing;

#pragma warning disable CA1515 // Consider making public types internal
public class PlayerInfoDto
{
    public required int PlayerId { get; set; }
    public required string Nick { get; set; }
}
