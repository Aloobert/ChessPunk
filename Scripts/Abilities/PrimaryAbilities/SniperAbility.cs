using UnityEngine;

[CreateAssetMenu(fileName = "New Sniper Ability", menuName = "Abilities/Sniper Ability")]
public class SniperAbility : AbilityBase
{
    public float tileSpacing = 1.0f; // Adjust as needed

    public override void ActivateAbility(GameObject user)
    {
        if (!IsAbilityReady())
        {
            Debug.Log($"{abilityName} is on cooldown.");
            return;
        }

        Vector2 origin = user.transform.position;

        // Retrieve the player's facing direction from the PlayerController.
        PlayerController controller = user.GetComponent<PlayerController>();
        if (controller == null)
        {
            Debug.LogError("PlayerController not found on user.");
            return;
        }

        // Use the facingDirection property you added to PlayerController.
        Vector2 direction = controller.facingDirection.normalized;
        float rayDistance = range * tileSpacing;

        Debug.Log($"{abilityName} activated. Raycasting from {origin} in direction {direction} for distance {rayDistance}.");

        // Perform a 2D raycast.
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance);
        if (hit.collider != null)
        {
            // Instead of referencing Enemy, we reference BossController.
            BossController boss = hit.collider.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log($"{abilityName} hit {boss.name} for {damage} damage.");
            }
            else
            {
                Debug.Log($"{abilityName} raycast hit {hit.collider.name}, but it's not a Boss.");
            }
        }
        else
        {
            Debug.Log($"{abilityName} did not hit any target.");
        }

        // Play activation animation, if assigned.
        if (activationAnimation != null)
        {
            Animator animator = user.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(activationAnimation.name);
                Debug.Log($"Playing animation {activationAnimation.name}.");
            }
        }

        currentCooldown = cooldown;
    }
}
