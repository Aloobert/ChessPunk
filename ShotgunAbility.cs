using UnityEngine;
using System.Collections;

public class ShotgunAbility : PrimaryAbility
{
    public GameObject projectilePrefab; // assign your projectile prefab here
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

        // Normalize the direction
        Vector3 dirStraight = direction.normalized;
        // Create left and right directions by rotating 45°.
        Vector3 dirLeft = Quaternion.Euler(0, 0, 45f) * dirStraight;
        Vector3 dirRight = Quaternion.Euler(0, 0, -45f) * dirStraight;

        // Fire three projectiles concurrently.
        StartCoroutine(FireProjectile(dirStraight));
        StartCoroutine(FireProjectile(dirLeft));
        StartCoroutine(FireProjectile(dirRight));

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FireProjectile(Vector3 direction)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector3 currentPos = transform.position;
        while (true)
        {
            Vector3 nextPos = currentPos + direction;
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm != null)
            {
                Node node = gm.GetNodeFromPosition(nextPos);
                if (!node.Walkable)
                    break;
            }
            Collider2D hit = Physics2D.OverlapPoint(nextPos);
            if (hit != null && hit.CompareTag("Boss"))
            {
                BossController boss = hit.GetComponent<BossController>();
                if (boss != null)
                    boss.TakeDamage(projectileDamage);
                break;
            }
            currentPos = nextPos;
            proj.transform.position = currentPos;
            yield return new WaitForSeconds(projectileTravelTime);
        }
        Destroy(proj);
    }
}
