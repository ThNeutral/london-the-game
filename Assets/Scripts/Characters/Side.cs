using System;
using UnityEngine;

public enum Side
{
    Player,
    AI
}

public static class SideExtensions
{
    public static void Next(ref this Side side)
    {
        var values = (Side[])Enum.GetValues(typeof(Side));
        int index = Array.IndexOf(values, side);
        int nextIndex = (index + 1) % values.Length;
        side = values[nextIndex];
    }
}