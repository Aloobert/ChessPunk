using UnityEngine;
using System.Collections;

public class ThreeRoundBurstAbility : PrimaryAbility
{
    public GameObject projectilePrefab; // assign your projectile prefab
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

        Vector3 dir = direction.normalized;
        // Fire three consecutive projectiles
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(FireProjectile(dir));
            yield return new WaitForSeconds(0.1f);
        }
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
