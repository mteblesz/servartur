using System.ComponentModel.DataAnnotations;

namespace servartur.Models.Incoming;

public class StartGameDto
{
    [Required]
    public required int RoomId { get; set; }
    [Required]
    public required bool AreMerlinAndAssassinInGame { get; set; }
    [Required]
    public required bool ArePercivalAndMorganaInGame { get; set; }
    [Required]
    public required bool AreOberonAndMordredInGame { get; set; }
}
