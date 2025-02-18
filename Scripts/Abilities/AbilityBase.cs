using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    [Header("General Settings")]
    public string abilityName;
    public AbilityClass abilityClass;
    public DamageType damageType;
    public Sprite icon;

    [Header("Combat Settings")]
    public int damage;
    public float cooldown;
    public float range; // For projectile or melee reach

    [Header("Projectile/Animation Settings")]
    public bool isMelee;                // If false, then projectile-based.
    public GameObject projectilePrefab; // The prefab for the projectile or melee hit effect.
    public int projectileCount;         // Number of projectiles (for shotgun, burst, etc.)
    public float spreadAngle;           // Total spread angle for multiple projectiles.
    public AnimationClip abilityAnimation; // Animation to play when the ability is used.

    // A method that will be overridden by concrete abilities to execute their behavior.
    public abstract void ExecuteAbility(GameObject user);
}
