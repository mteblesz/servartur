using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Servartur.Api.Endpoints.MathupHost.Models;

internal class StartGameRequest
{
    [FromRoute]
    [Required]
    public required Guid RoomId { get; init; }
}
