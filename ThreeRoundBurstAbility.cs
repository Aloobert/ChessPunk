using UnityEngine;
using System.Collections;

public class ThreeRoundBurstAbility : PrimaryAbility
{
    public int projectileDamage = 1;
    public float projectileTravelTime = 0.1f;

    void Awake()
    {
        cooldownRounds = 4;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        if (!IsReady())
            yield break;
        SetCooldown();

        Vector3 dirNormalized = direction.normalized;

        // Fire 3 consecutive projectiles.
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(FireProjectile(dirNormalized));
            // Minimal delay between spawns; they are fired "consecutively"
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FireProjectile(Vector3 direction)
    {
        Vector3 currentPos = transform.position;
        // For this ability, projectiles spawn regardless of immediate obstacle.
        // We simulate travel; if blocked, the projectile will immediately collide.
        while (true)
        {
            Vector3 nextPos = currentPos + direction;
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm != null)
            {
                Node node = gm.GetNodeFromPosition(nextPos);
                if (!node.Walkable)
                {
                    break;
                }
            }

            Collider2D hit = Physics2D.OverlapPoint(nextPos);
            if (hit != null && hit.CompareTag("Boss"))
            {
                BossController boss = hit.GetComponent<BossController>();
                if (boss != null)
                {
                    boss.TakeDamage(projectileDamage);
                }
                break;
            }
            // Advance projectile position.
            currentPos = nextPos;
            yield return new WaitForSeconds(projectileTravelTime);
        }
    }
}
