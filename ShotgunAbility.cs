using UnityEngine;
using System.Collections;

public class ShotgunAbility : PrimaryAbility
{
    public GameObject projectilePrefab; // Assign your ProjectileShotgun prefab here
    public int projectileDamage = 1;
    public float projectileTravelTime = 0.1f;

    void Awake()
    {
        cooldownRounds = 3;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        if (!IsReady())
            yield break;

        SetCooldown();

        // Normalize the base direction.
        Vector3 dirStraight = direction.normalized;
        Vector3 dirLeft = Quaternion.Euler(0, 0, 45f) * dirStraight;
        Vector3 dirRight = Quaternion.Euler(0, 0, -45f) * dirStraight;

        // Fire three projectiles concurrently.
        StartCoroutine(FireProjectile(dirStraight));
        StartCoroutine(FireProjectile(dirLeft));
        StartCoroutine(FireProjectile(dirRight));

        // Wait for a short time (adjust as needed) before finishing.
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FireProjectile(Vector3 direction)
    {
        // Instantiate the projectile prefab at the player's position.
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        // (Optional) You could set the projectile's parent to a dedicated container for organization.

        // Here we simulate the projectile traveling tile-by-tile.
        Vector3 currentPos = transform.position;
        while (true)
        {
            Vector3 nextPos = currentPos + direction;
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm != null)
            {
                Node node = gm.GetNodeFromPosition(nextPos);
                if (!node.Walkable)
                {
                    // The projectile hit a wall, pillar, or boundary.
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

            // Move the projectile to the next position.
            currentPos = nextPos;
            proj.transform.position = currentPos;
            yield return new WaitForSeconds(projectileTravelTime);
        }

        // Once the projectile stops, destroy it.
        Destroy(proj);
    }
}
