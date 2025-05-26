using System;
using UnityEngine;

public enum AbilityType
{
    MOVE,
    DAMAGE
}

[Serializable]
public class Ability
{
    public int MoveDistance = -1;
    public int AbilityDistance = 1;
    public int HitPointChange = 30;
    public AbilityType Type = AbilityType.DAMAGE;
}
