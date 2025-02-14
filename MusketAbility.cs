using UnityEngine;
using System.Collections;

public class MusketAbility : PrimaryAbility
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

        yield return StartCoroutine(FireProjectile(direction.normalized));
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
            if (hit != null)
            {
                if (hit.CompareTag("Boss"))
                {
                    BossController boss = hit.GetComponent<BossController>();
                    if (boss != null)
                    {
                        // Deal base damage
                        boss.TakeDamage(projectileDamage);

                        // Attempt to push the boss 1 tile backward
                        Vector3 pushPos = nextPos + direction; // one tile behind
                        Node pushNode = null;
                        if (gm != null)
                        {
                            pushNode = gm.GetNodeFromPosition(pushPos);
                        }

                        if (pushNode != null && !pushNode.Walkable)
                        {
                            // There's a wall/pillar behind; apply +1 extra damage to boss.
                            boss.TakeDamage(1);
                        }
                        else if (pushNode != null)
                        {
                            // Check if another enemy occupies pushPos (for chain reaction).
                            Collider2D secondaryHit = Physics2D.OverlapPoint(pushPos);
                            if (secondaryHit != null && secondaryHit.CompareTag("Boss"))
                            {
                                BossController secondBoss = secondaryHit.GetComponent<BossController>();
                                if (secondBoss != null)
                                {
                                    // Both take extra damage.
                                    secondBoss.TakeDamage(1);
                                    boss.TakeDamage(1);
                                }
                            }
                            else
                            {
                                // Push the boss to pushPos
                                boss.transform.position = pushPos;
                            }
                        }
                    }
                    break;
                }
            }

            currentPos = nextPos;
            yield return new WaitForSeconds(projectileTravelTime);
        }
    }
}
