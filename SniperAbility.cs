using UnityEngine;
using System.Collections;

public class SniperAbility : PrimaryAbility
{
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

        Vector3 dirNormalized = direction.normalized;
        yield return StartCoroutine(FireProjectile(dirNormalized));
    }

    IEnumerator FireProjectile(Vector3 direction)
    {
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
                {
                    boss.TakeDamage(projectileDamage);
                }
                break;
            }

            currentPos = nextPos;
            yield return new WaitForSeconds(projectileTravelTime);
        }
    }
}
