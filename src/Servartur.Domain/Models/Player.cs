using Servartur.Domain.Models.Enums;

namespace Servartur.Domain.Models;

public class Player
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Character Character { get; set; }
}
