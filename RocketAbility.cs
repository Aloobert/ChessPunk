using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RocketAbility : SpecialAbility
{
    public GameObject projectilePrefab; // assign a rocket projectile prefab
    public int baseDamage = 2;
    public int splashDamage = 1;
    public float projectileTravelTime = 0.1f;

    void Awake()
    {
        cooldownRounds = 6;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        if (!IsReady())
            yield break;
        SetCooldown();

        Vector3 dir = direction.normalized;
        yield return StartCoroutine(FireProjectile(dir));
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
                    boss.TakeDamage(baseDamage);

                // Splash damage to all 8 surrounding tiles.
                Vector3[] offsets = new Vector3[]
                {
                    new Vector3(1,0,0),
                    new Vector3(1,1,0),
                    new Vector3(0,1,0),
                    new Vector3(-1,1,0),
                    new Vector3(-1,0,0),
                    new Vector3(-1,-1,0),
                    new Vector3(0,-1,0),
                    new Vector3(1,-1,0)
                };
                foreach (Vector3 offset in offsets)
                {
                    Vector3 splashPos = nextPos + offset;
                    Collider2D splashHit = Physics2D.OverlapPoint(splashPos);
                    if (splashHit != null && splashHit.CompareTag("Boss"))
                    {
                        BossController splashBoss = splashHit.GetComponent<BossController>();
                        if (splashBoss != null)
                            splashBoss.TakeDamage(splashDamage);
                    }
                }
                break;
            }
            currentPos = nextPos;
            proj.transform.position = currentPos;
            yield return new WaitForSeconds(projectileTravelTime);
        }
        Destroy(proj);
    }
}
