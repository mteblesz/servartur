namespace servartur.Utils;

internal static class ListExtensionShuffle
{
    private static readonly Random Rng = new();
    /// <summary>
    /// Fisher-Yates shuffle extension for lists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        for (var i = 0; i < list.Count - 1; i++)
        {
            var pos = Rng.Next(i, list.Count);
            (list[i], list[pos]) = (list[pos], list[i]);
        }
    }
}
