using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlusBurstAbility : SpecialAbility
{
    public int damage = 3;

    void Awake()
    {
        cooldownRounds = 6;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        if (!IsReady())
            yield break;
        SetCooldown();

        // Damage orthogonally adjacent tiles: up, down, left, right.
        Vector3[] offsets = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };
        foreach (Vector3 offset in offsets)
        {
            Vector3 targetPos = transform.position + offset;
            Collider2D hit = Physics2D.OverlapPoint(targetPos);
            if (hit != null && hit.CompareTag("Boss"))
            {
                BossController boss = hit.GetComponent<BossController>();
                if (boss != null)
                    boss.TakeDamage(damage);
            }
        }
        yield return null;
    }
}
