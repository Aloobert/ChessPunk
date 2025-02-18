using UnityEngine;

[CreateAssetMenu(fileName = "New Generic Ability", menuName = "Abilities/Generic Ability")]
public class GenericAbility : AbilityBase
{
    [Header("Additional Settings")]
    public string effectDescription = "";
    // Only applicable for Special abilities.
    public float effectRadius = 0f;

    public override void ActivateAbility(GameObject user)
    {
        // For now, we simply log activation.
        Debug.Log($"{abilityName} ({abilityCategory}) activated with damage {damage} and cooldown {cooldown}.");
        // You could add different behavior based on category here.
        // For Passive abilities, this might be applied at the start of combat instead.
        currentCooldown = cooldown;
    }
}
