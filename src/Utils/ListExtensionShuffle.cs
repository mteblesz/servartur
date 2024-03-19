﻿namespace servartur.Utils;

public static class ListExtensionShuffle
{
    private static readonly Random _rng = new Random();
    /// <summary>
    /// Fisher-Yates shuffle extension for lists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int pos = _rng.Next(i, list.Count);
            (list[i], list[pos]) = (list[pos], list[i]);
        }
    }
}