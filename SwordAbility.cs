using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SwordAbility : SecondaryAbility
{
    public int damage = 2;

    void Awake()
    {
        cooldownRounds = 2;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        if (!IsReady())
            yield break;
        SetCooldown();

        // Assume the sword hits the tile immediately in the facing direction.
        Vector3 targetPos = transform.position + direction.normalized;
        Collider2D hit = Physics2D.OverlapPoint(targetPos);
        if (hit != null && hit.CompareTag("Boss"))
        {
            BossController boss = hit.GetComponent<BossController>();
            if (boss != null)
                boss.TakeDamage(damage);
        }
        yield return null;
    }
}
