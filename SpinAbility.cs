using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SpinAbility : SpecialAbility
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

        // Damage all 8 adjacent tiles.
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
