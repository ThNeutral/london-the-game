public class AbilityMessage
{
    public Ability Ability;
    public Side From;

    public static AbilityMessage Generate(Ability ability, Character actor)
    {
        return new AbilityMessage() { Ability = ability, From = actor.Allegiance };
    }
}

public interface IAbilityInteractable 
{ 
    public void InteractWithAbility(AbilityMessage message);
}