using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Servartur.Api.Endpoints.Matchup.Models;

public class LeaveRoomRequest
{
    [FromRoute]
    [Required]
    public required Guid RoomId { get; init; }

    [FromRoute]
    [Required]
    public required Guid PlayerId { get; init; }
}
