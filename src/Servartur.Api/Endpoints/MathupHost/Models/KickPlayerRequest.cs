using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Servartur.Api.Endpoints.MathupHost.Models;

internal class KickPlayerRequest
{
    [FromRoute]
    [Required]
    public required Guid RoomId { get; init; }

    [FromRoute]
    [Required]
    public required Guid PlayerId { get; init; }
}
