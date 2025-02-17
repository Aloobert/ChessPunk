using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WhipAbility : SecondaryAbility
{
    public int damage = 1;

    void Awake()
    {
        cooldownRounds = 3;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        if (!IsReady())
            yield break;
        SetCooldown();

        Vector3 dir = direction.normalized;
        // Whip attacks up to 3 tiles ahead.
        for (int i = 1; i <= 3; i++)
        {
            Vector3 targetPos = transform.position + dir * i;
            Collider2D hit = Physics2D.OverlapPoint(targetPos);
            if (hit != null && hit.CompareTag("Boss"))
            {
                BossController boss = hit.GetComponent<BossController>();
                if (boss != null)
                    boss.TakeDamage(damage);
                break; // Stop after hitting the first enemy.
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
