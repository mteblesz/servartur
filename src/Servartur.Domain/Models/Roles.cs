namespace Servartur.Domain.Models;

internal static class Roles
{
    public static IReadOnlyCollection<Character> Good = [
        Character.GoodKnight,
        Character.Merlin,
        Character.Percival ];

    public static IReadOnlyCollection<Character> Evil = [
        Character.EvilEntity,
        Character.Assassin,
        Character.Morgana,
         Character.Mordred,
        Character.Oberon ];
}
