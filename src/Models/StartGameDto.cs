using System.ComponentModel.DataAnnotations;

namespace servartur.Models;

public class StartGameDto
{
    [Required]
    public int RoomId { get; set; }
    [Required]
    public bool AreMerlinAndAssassinInGame { get; set; }
    [Required]
    public bool ArePercivalAreMorganaInGame { get; set; }
    [Required]
    public bool AreOberonAndMordredInGame { get; set; }
}
