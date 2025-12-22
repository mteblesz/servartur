using System.ComponentModel.DataAnnotations;

namespace Servartur.Api.Endpoints.Matchup.Models;

internal class CreatePlayerRequest
{
    [Required]
    [MaxLength(32)]
    public required string Name { get; init; }

    [Required]
    public required Guid RoomId { get; init; }
}
