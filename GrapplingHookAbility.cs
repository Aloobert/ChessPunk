using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GrapplingHookAbility : SecondaryAbility
{
    public int damage = 2;

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
        Vector3 currentPos = transform.position;
        Vector3 lastValidPos = currentPos;
        bool enemyEncountered = false;
        Collider2D enemyCollider = null;

        while (true)
        {
            Vector3 nextPos = currentPos + dir;
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm != null)
            {
                Node node = gm.GetNodeFromPosition(nextPos);
                if (!node.Walkable)
                {
                    // Pillar encountered: stop one tile before.
                    break;
                }
            }
            Collider2D hit = Physics2D.OverlapPoint(nextPos);
            if (hit != null && hit.CompareTag("Boss"))
            {
                enemyEncountered = true;
                enemyCollider = hit;
                lastValidPos = currentPos; // the tile before enemy
                break;
            }
            lastValidPos = nextPos;
            currentPos = nextPos;
        }

        if (enemyEncountered && enemyCollider != null)
        {
            // Pull enemy to one tile adjacent to the player.
            Vector3 pullPos = transform.position + dir;
            enemyCollider.transform.position = pullPos;
            BossController boss = enemyCollider.GetComponent<BossController>();
            if (boss != null)
                boss.TakeDamage(damage);
        }
        else
        {
            // Otherwise, pull the player to the last valid position.
            transform.position = lastValidPos;
        }
        yield return null;
    }
}
