using System.Collections;
using UnityEngine;

/// <summary>
/// The base Ability class provides shared functionality for all abilities,
/// such as cooldown management. Every ability must implement the UseAbility method.
/// </summary>
public abstract class Ability : MonoBehaviour
{
    [Tooltip("Number of rounds before this ability can be used again.")]
    public int cooldownRounds;

    // Tracks how many rounds remain until this ability is ready.
    protected int currentCooldown = 0;

    /// <summary>
    /// Checks if the ability is ready to be used.
    /// </summary>
    public bool IsReady()
    {
        return currentCooldown <= 0;
    }

    /// <summary>
    /// Sets the ability on cooldown.
    /// </summary>
    public void SetCooldown()
    {
        currentCooldown = cooldownRounds;
    }

    /// <summary>
    /// Reduces the current cooldown by 1 round. Call this once per round.
    /// </summary>
    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
            currentCooldown--;
    }

    /// <summary>
    /// Each ability must implement its own behavior when used.
    /// The direction parameter can be used for targeting.
    /// </summary>
    public abstract IEnumerator UseAbility(Vector3 direction);
}

/// <summary>
/// PrimaryAbility is the base for all primary (active) abilities.
/// Additional primary-specific properties or methods can be added here if needed.
/// </summary>
public abstract class PrimaryAbility : Ability
{
    // For now, no extra functionality beyond the base Ability class.
}

/// <summary>
/// SecondaryAbility is the base for all secondary abilities.
/// </summary>
public abstract class SecondaryAbility : Ability
{
    // Add secondary-specific functionality here if needed.
}

/// <summary>
/// SpecialAbility is the base for all special abilities.
/// </summary>
public abstract class SpecialAbility : Ability
{
    // Add special-specific functionality here if needed.
}

/// <summary>
/// PassiveAbility is for abilities that are continuously in effect.
/// Typically, these do not need to be "used" actively, so we provide a default implementation.
/// </summary>
public abstract class PassiveAbility : Ability
{
    /// <summary>
    /// Passive abilities typically do not use the active UseAbility method.
    /// This default implementation does nothing.
    /// </summary>
    public override IEnumerator UseAbility(Vector3 direction)
    {
        yield break;
    }

    // Concrete passive abilities can override OnEnable to apply their effect.
}
