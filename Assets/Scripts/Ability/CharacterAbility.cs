using System;

[Serializable]
public class CharacterAbility
{
    public int Damage = 30;
    public int Distance = 3;
    public bool CanInteractWithAlly = false;
    public bool CanInteractWithEnemy = true;
}