using UnityEngine;
using System.Collections;

public class SniperAbility : PrimaryAbility
{
    public GameObject projectilePrefab; // assign your projectile prefab here
    public int projectileDamage = 2;
    public float projectileTravelTime = 0.1f;

    void Awake()
    {
        cooldownRounds = 2;
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
