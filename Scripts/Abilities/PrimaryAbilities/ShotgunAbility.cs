using UnityEngine;

[CreateAssetMenu(fileName = "NewShotgunAbility", menuName = "Abilities/Shotgun Ability")]
public class ShotgunAbility : AbilityBase
{
    public override void ExecuteAbility(GameObject user)
    {
        // Debug feedback to verify execution.
        Debug.Log($"{abilityName} executed by {user.name}");

        // Example logic: Spawn multiple projectiles in a cone.
        if (!isMelee && projectilePrefab != null)
        {
            float angleStep = spreadAngle / (projectileCount - 1);
            float startingAngle = -spreadAngle / 2f;

            for (int i = 0; i < projectileCount; i++)
            {
                // Calculate rotation for each projectile.
                float currentAngle = startingAngle + (angleStep * i);
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

                // Instantiate projectile at user's position.
                GameObject projectile = Instantiate(projectilePrefab, user.transform.position, user.transform.rotation * rotation);
                // You can add further logic for projectile movement, damage, etc.
            }
        }

        // Optionally, trigger an animation.
        if (abilityAnimation != null)
        {
            Animator animator = user.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(abilityAnimation.name);
            }
        }
    }
}
