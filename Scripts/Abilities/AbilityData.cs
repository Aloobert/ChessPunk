using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability Data", order = 0)]
public class AbilityData : AbilityBase
{
    public bool isPassive;

    // Active ability fields
    public int damage;
    public int cooldown;
    public int projectileCount;
    public float projectileDelay;
    public float knockbackDistance;
    public int knockbackDamage;
    public int[] projectileDirections;

    // Passive ability fields (nested enum for editor usage)
    public enum PassiveType
    {
        HealthAura,
        DamageAura,
        DefenseAura,
        SpeedAura
    }
    public PassiveType passiveType;
    public float passiveMagnitude;

    // Implement abstract methods (even if they’re empty for now)
    public override void ActivateAbility(GameObject target)
    {
        Debug.Log("Activating ability: " + abilityName);
    }

    public override void EndTurn()
    {
        Debug.Log("Ending turn for ability: " + abilityName);
    }
}
