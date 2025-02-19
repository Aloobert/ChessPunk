using UnityEngine;

[CreateAssetMenu(fileName = "NewGenericAbility", menuName = "Abilities/Generic Ability", order = 1)]
public class GenericAbility : AbilityBase
{
    public override void ActivateAbility(GameObject target)
    {
        // Implement your ability logic here.
        Debug.Log("Activating generic ability '" + abilityName + "' on " + target.name);
    }

    public override void EndTurn()
    {
        // Implement your end-of-turn logic here (e.g., reducing cooldowns).
        Debug.Log("Ending turn for generic ability '" + abilityName + "'.");
    }
}
