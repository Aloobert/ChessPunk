using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    public string abilityName;
    public abstract void ActivateAbility(GameObject target);
    public abstract void EndTurn();
}
