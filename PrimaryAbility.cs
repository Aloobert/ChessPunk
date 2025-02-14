using UnityEngine;
using System.Collections;

public abstract class PrimaryAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    public int cooldownRounds = 3; // default cooldown; override in derived scripts
    protected int currentCooldown = 0;

    // Determines if the ability is ready to use.
    public bool IsReady()
    {
        return currentCooldown <= 0;
    }

    // Resets the ability cooldown.
    protected void SetCooldown()
    {
        currentCooldown = cooldownRounds;
    }

    // Called at the end of a round to decrement cooldown.
    public void DecrementCooldown()
    {
        if (currentCooldown > 0)
            currentCooldown--;
    }

    // Each primary ability must implement how it fires.
    // direction: the intended firing direction (based on player facing, etc.)
    public abstract IEnumerator UseAbility(Vector3 direction);
}
